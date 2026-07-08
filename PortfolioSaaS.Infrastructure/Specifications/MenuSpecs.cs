using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class MenuSpecs
{
    public static Specification<Menu> IncludeMenuItems(Guid id)
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(m => m.Id == id);
        spec.Query.Include(m => m.MenuItems).ThenInclude(mi => mi.SubMenuItems);
        return spec;
    }
    public static Specification<Menu> IncludeMenuItems(MenuType type)
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(m => m.Type == type);
        spec.Query.Include(m => m.MenuItems).ThenInclude(mi => mi.SubMenuItems);
        return spec;
    }
}
