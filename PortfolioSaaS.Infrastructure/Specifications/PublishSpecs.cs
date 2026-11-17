using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PublishSpecs
{
    public static Specification<Page> PagesToPublish()
    {
        var spec = new Specification<Page>();
        spec.Query.Where(x => x.ToPublish);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.Media).AsSplitQuery();
        return spec;
    }
    public static Specification<Menu> MenusToPublish()
    {
        var spec = new Specification<Menu>();
        spec.Query.Where(x => x.ToPublish);
        spec.Query.Include(x => x.MenuItems).ThenInclude(x => x.SubMenuItems).AsSplitQuery();
        return spec;
    }
    public static Specification<ThemeConfig> ThemeConfigsToPublish()
    {
        var spec = new Specification<ThemeConfig>();
        spec.Query.Where(x => x.ToPublish);
        return spec;
    }
    public static Specification<PageSnapshot> PageSnapshot(Guid versionId, Guid pageId)
    {
        var spec = new Specification<PageSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalPageId == pageId);
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.Media).AsSplitQuery();
        return spec;
    }
    public static Specification<MenuSnapshot> MenuSnapshot(Guid versionId, Guid menuId)
    {
        var spec = new Specification<MenuSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalMenuId == menuId);
        spec.Query.Include(x => x.MenuItems).ThenInclude(x => x.SubMenuItems).AsSplitQuery();
        return spec;
    }
    public static Specification<ThemeConfigSnapshot> ThemeConfigSnapshot(Guid versionId, Guid themeConfigId)
    {
        var spec = new Specification<ThemeConfigSnapshot>();
        spec.Query.Where(p => p.PublishedVersionId == versionId);
        spec.Query.Where(p => p.OriginalThemeConfigId == themeConfigId);
        return spec;
    }
}
