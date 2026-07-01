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
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector));
        CreateMap<PublishedSnapshotPage, PublishedSnapshotPageDto>();
    }
}
