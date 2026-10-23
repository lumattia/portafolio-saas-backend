using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class MediaSpecs
{
    public static PagedSpecification<Media> Page(MediaFilterRequest request, PagedParameters parameters, Guid tenantId)
    {
        var spec = new PagedSpecification<Media>(parameters);
        spec.Query.Where(m => m.TenantId == tenantId);
        spec.Query.Search(x => x.FileName.ToLower(), $"%{request.SearchTerm?.ToLower()}%", request.SearchTerm != null);
        spec.Query.Where(x => x.MediaType.ToString() == request.MediaType,!string.IsNullOrWhiteSpace(request.MediaType));
        spec.Query.OrderByDescending(x => x.UpdatedAt);
        return spec;
    }
}
