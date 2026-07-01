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
        spec.Query.Search(x => x.Name, request.Name!, request.Name != null);
        return spec;
    }
}
