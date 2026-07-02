using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class MenuService(
    TenantBaseRepository<MenuItem> menuRepository,
    TenantBaseRepository<Page> pageRepository,
    BaseRepository<Tenant> tenantRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<MenuItem> _menuRepository = menuRepository;
    private readonly TenantBaseRepository<Page> _pageRepository = pageRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<MenuDto?> CreateAsync(MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        if (tenantId == null)
            return null;

        var page = await _pageRepository.FirstOrDefaultBySpecAsync(PageSpecs.GetByIdentifierIncludeSection((request.PageSlug)));
        if (page == null)
        {
            // Create page if not found
            page = new Page
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Title = request.Text,
                Slug = request.PageSlug,
                MetaDescription = string.Empty,
                Disabled = false
            };
            await _pageRepository.SaveAsync(page);
        }

        var menuItem = new MenuItem
        {
            Id = request.Id ?? Guid.NewGuid(),
            TenantId = tenantId.Value,
            Text = request.Text,
            PageId = page.Id,
            ExternalUrl = request.ExternalUrl,
            IsExternal = request.IsExternal,
            Order = request.Order
        };

        await _menuRepository.SaveAsync(menuItem);

        var dto = _mapper.Map<MenuDto>(menuItem);
        dto.PageSlug = page.Slug;

        return dto;
    }

    public async Task<MenuDto?> UpdateAsync(Guid id, MenuRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        if (tenantId == null)
            return null;

        var menuItem = await _menuRepository.GetByIdAsync(id);
        if (menuItem == null)
            return null;

        var page = await _pageRepository.FirstOrDefaultBySpecAsync(PageSpecs.GetByIdentifierIncludeSection((request.PageSlug)));
        if (page == null)
        {
            // Create page if not found
            page = new Page
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Title = request.Text,
                Slug = request.PageSlug,
                MetaDescription = string.Empty,
                Disabled = false
            };
            await _pageRepository.SaveAsync(page);
        }

        menuItem.Text = request.Text;
        menuItem.PageId = page.Id;
        menuItem.ExternalUrl = request.ExternalUrl;
        menuItem.IsExternal = request.IsExternal;
        menuItem.Order = request.Order;

        await _menuRepository.SaveAsync(menuItem);

        var dto = _mapper.Map<MenuDto>(menuItem);
        dto.PageSlug = page.Slug;

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

    public async Task<List<MenuDto>> GetAllAsync(Boolean anonymous = false)
    {
        if (!_tenantContext.IsResolved)
            return [];

        var tenant = await _tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeMenu(_tenantContext.CurrentTenantId!.Value));

        var menuItems = anonymous ? tenant.MenuItems.Where(m => m.Page!.IsPublished) : tenant.MenuItems;
        return _mapper.Map<List<MenuDto>>(menuItems.OrderBy(m => m.Order));
    }

    public async Task ReorderAsync(List<Guid> menuIds)
    {
        if (!_tenantContext.IsAuthenticated)
            return;

        var tenant = await _tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeMenu(_tenantContext.CurrentTenantId!.Value));

        var allMenuItems = tenant!.MenuItems.ToList();

        // Ignore the case that some menuIds are not in the tenant's menu items
        var outListMenuCount = 0;
        foreach (var menuItem in allMenuItems)
        {
            var index = menuIds.IndexOf(menuItem.Id);
            if (index >= 0)
            {
                menuItem.Order = index;
            }
            else
            {
                outListMenuCount++;
                // Menu item is not in the provided list, assign order after the last one
                menuItem.Order = menuIds.Count + outListMenuCount;
            }
            await _menuRepository.SaveAsync(menuItem);
        }
    }
}
