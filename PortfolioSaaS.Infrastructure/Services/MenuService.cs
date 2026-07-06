using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class MenuService(
    TenantBaseRepository<Menu> menuRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<Menu> _menuRepository = menuRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<MenuDto?> CreateAsync(MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        if (tenantId == null)
            return null;

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
                Order = index
            })]
        };

        await _menuRepository.SaveAsync(menu);

        var dto = _mapper.Map<MenuDto>(menu);

        return dto;
    }

    public async Task<MenuDto?> UpdateAsync(Guid id, MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var menu = await _menuRepository.FirstOrDefaultBySpecAsync(MenuSpecs.IncludeMenuItems(id));
        if (menu == null)
            return null;
        menu.MenuItems = [.. request.MenuItems.Select((mi, index) => new MenuItem
        {
            Id = mi.Id ?? Guid.NewGuid(),
            Text = mi.Text,
            Url = mi.Url,
            Order = index
        })];
        menu.ToPublish = true;
        await _menuRepository.SaveAsync(menu);

        var dto = _mapper.Map<MenuDto>(menu);

        return dto;
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

    public async Task<MenuDto?> GetAsync(MenuType type)
    {
        if (!_tenantContext.IsResolved)
            return null;

        var menu = await _menuRepository.FirstOrDefaultBySpecAsync(MenuSpecs.GetByType(type));

        return _mapper.Map<MenuDto>(menu);
    }
}
