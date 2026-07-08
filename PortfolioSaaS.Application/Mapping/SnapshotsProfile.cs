using AutoMapper;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PublishedSnapshotProfile : Profile
{
    public PublishedSnapshotProfile()
    {
        CreateMap<ThemeConfigSnapshot, ThemeConfigSnapshotDto>();
        CreateMap<Section, SectionSnapshot>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.OriginalSectionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.Where(s => !s.IsDeleted)));
        CreateMap<MenuItem, MenuItemSnapshot>();
    }
}
