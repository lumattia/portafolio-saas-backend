using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PageSpecs
{
    public static Specification<Page> GetByIdentifierIncludeSection(string identifier)
    {
        var spec = new Specification<Page>();
        if (Guid.TryParse(identifier, out var id))
            spec.Query.Where(x => x.Id == id);
        else
            spec.Query.Where(x => x.Slug == identifier);

        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
}
