using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class SnapshotsSpecs
{
    public static Specification<PageSnapshot> GetPage(string identifier, bool IsAuthenticated, int? versionNumber)
    {
        var spec = new Specification<PageSnapshot>();
        if (Guid.TryParse(identifier, out var id))
            spec.Query.Where(p => p.OriginalPageId == id);
        else
            spec.Query.Where(p => p.Slug == identifier);
        spec.Query.Where(p => !p.Disabled && !p.IsDeleted, !IsAuthenticated);
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.OrderByDescending(p => p.PublishedVersion.Number).Take(1);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
    public static Specification<PageSnapshot> GetPage(Guid versionId, Guid pageId)
    {
        var spec = new Specification<PageSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalPageId == pageId);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
    public static Specification<MenuSnapshot> GetMenuSnapshots(int? versionNumber)
    {
        var spec = new Specification<MenuSnapshot>();
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.PostProcessingAction(items =>
            items
                .GroupBy(p => p.Type)
                .Select(g => g.OrderByDescending(p => p.PublishedVersion.Number).First()));
        return spec;
    }
}
