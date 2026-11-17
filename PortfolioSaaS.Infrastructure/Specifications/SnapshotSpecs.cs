using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class SnapshotSpecs
{
    public static Specification<PageSnapshot> PageSnapshot(string identifier, bool IsAuthenticated, int? versionNumber)
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
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.Media).AsSplitQuery();
        return spec;
    }
    public static Specification<MenuSnapshot> MenuSnapshot(MenuType menuType, int? versionNumber)
    {
        var spec = new Specification<MenuSnapshot>();
        spec.Query.Where(p => p.Type == menuType);
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.OrderByDescending(p => p.PublishedVersion.Number).Take(1);
        spec.Query.Include(x => x.MenuItems).ThenInclude(x => x.SubMenuItems).AsSplitQuery();
        return spec;
    }
    public static Specification<ThemeConfigSnapshot> ThemeConfigSnapshot(int? versionNumber)
    {
        var spec = new Specification<ThemeConfigSnapshot>();
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.OrderByDescending(p => p.PublishedVersion.Number).Take(1);
        return spec;
    }
}
