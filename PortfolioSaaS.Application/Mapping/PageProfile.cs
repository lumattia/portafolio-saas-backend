using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<Page, PageDto>();
        CreateMap<Page, PageDetailDto>()
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections.Where(s=>s.ParentSectionId == null).OrderBy(s => s.Order)));
        CreateMap<Section, SectionDto>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.OrderBy(s => s.Order)));
    }
}
