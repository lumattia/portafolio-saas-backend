using AutoMapper;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class TableProfile : Profile
{
    public TableProfile()
    {
        CreateMap<Page, PageDto>()
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => !src.Disabled));
        CreateMap<PageSnapshot, SnapshotPageDto>()
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => !src.Disabled))
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.PublishedVersion.Number));
    }
}
