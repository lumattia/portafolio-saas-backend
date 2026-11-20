using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class PageSpecs
{
    public static PagedSpecification<Page> Page(PagedParameters parameters, PageFilters filters)
    {
        var spec = new PagedSpecification<Page>(parameters);
        spec.Query.Where(x => x.Slug.Contains(filters.Slug!), !string.IsNullOrWhiteSpace(filters.Slug));
        spec.Query.Where(x => x.Title.Contains(filters.Title!), !string.IsNullOrWhiteSpace(filters.Title));
        spec.Query.Where(x => x.ToPublish == filters.ToPublish, filters.ToPublish.HasValue);
        spec.Query.Where(x => x.IsPublished == filters.IsPublished, filters.IsPublished.HasValue);
        spec.Query.Where(x => x.IsDeleted == filters.IsDeleted, filters.IsDeleted.HasValue);
        spec.Query.Where(x => x.Disabled == filters.Disabled, filters.Disabled.HasValue);
        spec.OrderExpression = parameters.SortBy?.ToLowerInvariant() switch
        {
            "slug"  => x => x.Slug,
            "title" => x => x.Title,
            _       => null
        };
        return spec;
    }

    public static Specification<Page> GetByIdentifierIncludeSection(string identifier)
    {
        var spec = new Specification<Page>();
        if (Guid.TryParse(identifier, out var id))
            spec.Query.Where(x => x.Id == id);
        else
            spec.Query.Where(x => x.Slug == identifier);

        spec.Query.Include(x => x.Sections).ThenInclude(x => x.SectionTemplate).AsSplitQuery();
        spec.Query.Include(x => x.Sections).ThenInclude(x => x.Media).AsSplitQuery();
        return spec;
    }
}