using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class TenantSpecs
{
    public static Specification<Tenant> IncludeTheme(Guid id)
    {
        var spec = new Specification<Tenant>();

        spec.Query
            .Where(x => x.Id == id)
            .Include(x => x.ThemeConfig);

        return spec;
    }

    public static Specification<Tenant> IncludeMenu(Guid id)
    {
        var spec = new Specification<Tenant>();

        spec.Query
            .Where(x => x.Id == id)
            .Include(x => x.MenuItems).ThenInclude(x => x.Page);

        return spec;
    }
}
