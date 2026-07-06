using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class SnapshotsSpecs
{
    // Get valid version
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
    public static Specification<MenuSnapshot> GetMenu(MenuType menuType, int? versionNumber)
    {
        var spec = new Specification<MenuSnapshot>();
        spec.Query.Where(p => p.Type == menuType);
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.OrderByDescending(p => p.PublishedVersion.Number).Take(1);
        return spec;
    }
    public static Specification<ThemeConfigSnapshot> GetThemeConfig(int? versionNumber)
    {
        var spec = new Specification<ThemeConfigSnapshot>();
        spec.Query.Where(p => p.PublishedVersion.Number <= versionNumber, versionNumber.HasValue);
        spec.Query.OrderByDescending(p => p.PublishedVersion.Number).Take(1);
        return spec;
    }
    // Get toPublish original entity
    public static Specification<Page> PageToPublish()
    {
        var spec = new Specification<Page>();
        spec.Query.Where(x => x.ToPublish);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
    public static Specification<Menu> MenuToPublish()
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(x => x.ToPublish);
        spec.Query.Include(x => x.MenuItems);
        return spec;
    }
    public static Specification<ThemeConfig> ThemeConfigToPublish()
    {
        var spec = new Specification<ThemeConfig>();
        spec.Query.Where(x => x.ToPublish);
        return spec;
    }
    // Get specific Version
    public static Specification<PageSnapshot> GetPage(Guid versionId, Guid pageId)
    {
        var spec = new Specification<PageSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalPageId == pageId);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        return spec;
    }
    public static Specification<MenuSnapshot> GetMenu(Guid versionId, Guid menuId)
    {
        var spec = new Specification<MenuSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalMenuId == menuId);
        return spec;
    }
    public static Specification<ThemeConfigSnapshot> GetThemeConfig(Guid versionId, Guid themeConfigId)
    {
        var spec = new Specification<ThemeConfigSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalThemeConfigId == themeConfigId);
        return spec;
    }
}
