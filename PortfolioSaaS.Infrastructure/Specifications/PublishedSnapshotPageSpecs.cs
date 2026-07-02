using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PublishedSnapshotPageSpecs
{
    public static Specification<PublishedSnapshotPage> GetByIdentifierIncludeSection(string identifier, bool hideDisabled)
    {
        var spec = new Specification<PublishedSnapshotPage>();
        if (hideDisabled) spec.Query.Where(p => !p.Disabled);
        if (Guid.TryParse(identifier, out var id))
            spec.Query.Where(p => p.OriginalPageId == id);
        else
            spec.Query.Where(p => p.Slug == identifier);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
}
