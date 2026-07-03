using AutoMapper;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class ThemeConfigProfile : Profile
{
    public ThemeConfigProfile()
    {
        CreateMap<ThemeConfig, ThemeConfigDto>();
    }
}
