using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class SectionSpecs : Specification<Section>
{
    public static SectionSpecs ByPageId(Guid pageId)
    {
        var spec = new SectionSpecs();
        spec.Query.Where(x => x.PageId == pageId);
        return spec;
    }

    public static SectionSpecs ByPageIdWithMaxOrder(Guid pageId)
    {
        var spec = new SectionSpecs();
        spec.Query.Where(x => x.PageId == pageId).OrderByDescending(x => x.Order);
        return spec;
    }
}
