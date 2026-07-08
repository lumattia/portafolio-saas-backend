using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.Mapping;

public class RendererProfile : Profile
{
    public RendererProfile()
    {
        //page
        CreateMap<Page, PageRenderer>()
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections.Where(s=>s.ParentSectionId == null).OrderBy(s => s.Order)));
        CreateMap<Section, SectionRenderer>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.OrderBy(s => s.Order)));
        //menu
        CreateMap<Menu, MenuRenderer>()
            .ForMember(dest => dest.MenuItems, opt => opt.MapFrom(src => src.MenuItems.Where(m => m.ParentMenuItemId == null).OrderBy(s => s.Order)));
        CreateMap<MenuItem, MenuItemRenderer>()
            .ForMember(dest => dest.SubMenuItems, opt => opt.MapFrom(src => src.SubMenuItems.OrderBy(s => s.Order)));

        // page snapshot
        CreateMap<PageSnapshot, PageRenderer>()
            .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections.Where(s => s.ParentSectionId == null).OrderBy(s => s.Order)));
        CreateMap<SectionSnapshot, SectionRenderer>()
            .ForMember(dest => dest.ComponentSelector, opt => opt.MapFrom(src => src.SectionTemplate!.ComponentSelector))
            .ForMember(dest => dest.ContentJson, opt => opt.MapFrom(src => JsonDocument.Parse(src.ContentJson)))
            .ForMember(dest => dest.SubSections, opt => opt.MapFrom(src => src.SubSections.OrderBy(s => s.Order)))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(_ => true));
        // menu snapshot
        CreateMap<MenuSnapshot, MenuRenderer>();
    }
}
