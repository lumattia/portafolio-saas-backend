using Ardalis.Specification;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class MenuService(
    BaseRepository<Menu> menuRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly BaseRepository<Menu> _menuRepository = menuRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<MenuRenderer?> GetByTypeAsync(MenuType type)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;
        var spec = MenuSpecs.IncludeMenuItems(type);
        var menu = await _menuRepository.FirstOrDefaultBySpecAsync(spec);

        if (menu == null) return null;

        return _mapper.Map<MenuRenderer>(menu);
    }

    public async Task<MenuRenderer?> CreateAsync(MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        if (tenantId == null) return null;

        var menu = new Menu
        {
            Id = request.Id ?? Guid.NewGuid(),
            TenantId = tenantId.Value,
            Type = request.Type,
            MenuItems = [.. request.MenuItems.Select((mi, index) => new MenuItem
            {
                Id = mi.Id ?? Guid.NewGuid(),
                Text = mi.Text,
                Url = mi.Url,
                Order = index,
                ParentMenuItemId = mi.ParentMenuItemId
            })]
        };

        await _menuRepository.SaveAsync(menu);

        return _mapper.Map<MenuRenderer>(menu);
    }

    public async Task<MenuRenderer?> UpdateAsync(Guid id, MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var menu = await _menuRepository.GetUniqueBySpecAsync(MenuSpecs.IncludeMenuItems(id));

        menu.ToPublish = true;
        menu.MenuItems = [.. request.MenuItems.Select((mi, index) => new MenuItem
        {
            Id = mi.Id ?? Guid.NewGuid(),
            Text = mi.Text,
            Url = mi.Url,
            Order = index,
            ParentMenuItemId = mi.ParentMenuItemId,
            MenuId = menu.Id
        })];
        await _menuRepository.SaveAsync(menu);
        return _mapper.Map<MenuRenderer>(menu);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!_tenantContext.IsAuthenticated)
            return false;

        var menuItem = await _menuRepository.GetByIdAsync(id);
        if (menuItem == null)
            return false;

        await _menuRepository.DeleteAsync(menuItem);
        return true;
    }
}
