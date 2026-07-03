using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PublishedSnapshotProfile : Profile
{
    public PublishedSnapshotProfile()
    {
        CreateMap<PageSnapshot, PageSnapshotDto>()
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections.Where(s => s.ParentSectionId == null).OrderBy(s => s.Order)));
        CreateMap<SectionSnapshot, SectionSnapshotDto>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.OrderBy(s => s.Order)));
        CreateMap<MenuSnapshot, MenuSnapshotDto>()
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)));

        CreateMap<Section, SectionSnapshot>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.OriginalSectionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.Where(s => !s.IsDeleted)));
    }
}
