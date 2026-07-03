using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class MenuMappingProfile : Profile
{
    public MenuMappingProfile()
    {
        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<Menu, MenuDto>();
    }
}
