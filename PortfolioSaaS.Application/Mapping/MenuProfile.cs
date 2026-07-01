using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class MenuMappingProfile : Profile
{
    public MenuMappingProfile()
    {
        CreateMap<MenuItem, MenuDto>()
            .ForMember(dest => dest.PageSlug, opt => opt.MapFrom(src => src.Page != null ? src.Page.Slug : null));
    }
}
