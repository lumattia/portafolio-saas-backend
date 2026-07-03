using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PublishedVersionSpecs
{
    public static Specification<PublishedVersion> GetLatestVersion()
    {
        var spec = new Specification<PublishedVersion>();
        spec.Query.OrderByDescending(v => v.Number).Take(1);
        return spec;
    }
}
