using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PublishedSnapshotProfile : Profile
{
    public PublishedSnapshotProfile()
    {
        CreateMap<PublishedSnapshotPage, PublishedSnapshotPageDto>()
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections.Where(s => s.ParentSectionId == null).OrderBy(s => s.Order)));
        CreateMap<PublishedSnapshotSection, PublishedSnapshotSectionDto>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.OrderBy(s => s.Order)));
    }
}
