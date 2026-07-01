using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class PageSpecs : Specification<Page>
{

    public static PageSpecs GetBySlug(string slug)
    {
        var spec = new PageSpecs();

        spec.Query
            .Where(x => x.Slug == slug);

        return spec;
    }
    public static PageSpecs GetByIdentifierIncludeSection(string identifier)
    {
        var spec = new PageSpecs();
        if (Guid.TryParse(identifier, out var id))
            spec.Query.Where(x => x.Id == id);
        else
            spec.Query.Where(x => x.Slug == identifier);

        spec.Query.Include(x => x.Sections.OrderBy(s => s.Order)).ThenInclude(x => x.SectionTemplate);
        return spec;
    }
}
