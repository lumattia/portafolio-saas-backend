using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Application.DTOs.SectionTemplates;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class SectionTemplateSpecs
{
    public static PagedSpecification<SectionTemplate> Page(SectionTemplateFilterRequest request, PagedParameters param)
    {
        var spec = new PagedSpecification<SectionTemplate>(param);
        spec.Query.Where(x => x.CategoryTags.HasFlag(request.CategoryTags!.Value), request.CategoryTags != null);
        spec.Query.Search(x => x.Name.ToLower(), $"%{request.Name?.ToLower()}%", request.Name != null);
        return spec;
    }
     public static Specification<SectionTemplate> ByIds(List<Guid> ids)
    {
        var spec = new Specification<SectionTemplate>();
        spec.Query.Where(m => ids.Contains(m.Id));
        return spec;
    }
}
