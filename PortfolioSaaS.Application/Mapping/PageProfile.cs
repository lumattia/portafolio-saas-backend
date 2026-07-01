using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<Page, PageDto>();
        CreateMap<Page, PageDetailDto>();
        CreateMap<Section, SectionDto>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.SnapshotSectionId != null));
        CreateMap<PublishedSnapshotPage, PublishedSnapshotPageDto>();
    }
}
