using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class TenantSpecs : Specification<Tenant>
{
    public static TenantSpecs IncludeTheme(Guid id)
    {
        var spec = new TenantSpecs();

        spec.Query
            .Where(x => x.Id == id)
            .Include(x => x.ThemeConfig);

        return spec;
    }

    public static TenantSpecs IncludeMenu(Guid id)
    {
        var spec = new TenantSpecs();

        spec.Query
            .Where(x => x.Id == id)
            .Include(x => x.MenuItems).ThenInclude(x => x.Page);

        return spec;
    }
}
