using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class VersionSpecs
{
    public static Specification<PublishedVersion> GetLatestVersion()
    {
        var spec = new Specification<PublishedVersion>();
        spec.Query.OrderByDescending(v => v.Number).Take(1);
        return spec;
    }

    public static Specification<PublishedVersion> ById(Guid id)
    {
        var spec = new Specification<PublishedVersion>();
        spec.Query.Where(v => v.Id == id);
        return spec;
    }
}
