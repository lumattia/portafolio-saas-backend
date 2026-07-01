using AutoMapper;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class PublishedSnapshotProfile : Profile
{
    public PublishedSnapshotProfile()
    {
        CreateMap<PublishedSnapshotPage, PublishedSnapshotPageDto>();
        CreateMap<PublishedSnapshotSection, PublishedSnapshotSectionDto>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector));
    }
}
