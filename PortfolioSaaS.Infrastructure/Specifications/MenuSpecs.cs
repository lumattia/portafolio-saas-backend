using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class MenuSpecs
{
    public static Specification<Menu> IncludeMenuItems(Guid id)
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(m => m.Id == id);
        spec.Query.Include(m => m.MenuItems);
        return spec;
    }
    public static Specification<Menu> GetByType(MenuType? type)
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(m => m.Type == type!.Value, type.HasValue);
        spec.Query.Include(m => m.MenuItems);
        return spec;
    }
}
