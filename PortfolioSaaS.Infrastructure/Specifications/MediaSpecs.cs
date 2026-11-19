using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class MediaSpecs
{
    public static PagedSpecification<Media> Page(MediaFilterRequest request, PagedParameters parameters, Guid tenantId)
    {
        var spec = new PagedSpecification<Media>(parameters);
        spec.Query.Where(m => m.TenantId == tenantId);
        spec.Query.Search(x => x.FileName.ToLower(), $"%{request.SearchTerm?.ToLower()}%", request.SearchTerm != null);
        spec.Query.Where(x => x.MediaType.ToString() == request.MediaType,!string.IsNullOrWhiteSpace(request.MediaType));
        spec.OrderExpression = parameters.SortBy?.ToLowerInvariant() switch
        {
            "createdat"  => x => x.CreatedAt,
            "updatedat"  => x => x.UpdatedAt,
            "filename"   => x => x.FileName,
            "size"       => x => x.Size,
            "usagecount" => x => x.UsageCount,
            _            => null
        };
        return spec;
    }

    public static Specification<Media> ByIds(List<Guid> ids)
    {
        var spec = new Specification<Media>();
        spec.Query.Where(m => ids.Contains(m.Id));
        return spec;
    }
}
