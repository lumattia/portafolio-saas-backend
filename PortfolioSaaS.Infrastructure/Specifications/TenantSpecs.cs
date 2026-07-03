using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class TenantSpecs
{
    public static Specification<Tenant> IncludeVersion(Guid id)
    {
        var spec = new Specification<Tenant>();

        spec.Query
            .Where(x => x.Id == id)
            .Include(x => x.CurrentVersion);

        return spec;
    }
}
