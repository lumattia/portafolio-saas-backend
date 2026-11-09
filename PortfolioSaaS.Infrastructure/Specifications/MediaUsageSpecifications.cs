using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;

namespace PortfolioSaaS.Infrastructure.Specifications;

public interface IMediaUsageSpecification<T> where T : class, IMediaUsable
{
    Task<List<MediaUsageDetail>> GetUsageLocations(Guid mediaId, Guid tenantId, ApplicationDbContext context);
}

public class SectionMediaUsageSpecification : IMediaUsageSpecification<Section>
{
    public async Task<List<MediaUsageDetail>> GetUsageLocations(Guid mediaId, Guid tenantId, ApplicationDbContext context)
    {
        var sections = await context.Sections
            .Include(s => s.Page)
            .Where(s => s.MediaId == mediaId && s.Page!.TenantId == tenantId)
            .ToListAsync();

        return sections.ConvertAll(s => new MediaUsageDetail
        {
            EntityType = "Section",
            EntityId = s.Id,
            Title = s.Page?.Title ?? $"Section #{s.Id}",
            RoutePath = $"/{s.Page!.Slug}#draft-{s.Id}"
        });
    }
}

public class SectionSnapshotMediaUsageSpecification : IMediaUsageSpecification<SectionSnapshot>
{
    public async Task<List<MediaUsageDetail>> GetUsageLocations(Guid mediaId, Guid tenantId, ApplicationDbContext context)
    {
        var snapshots = await context.SectionSnapshots
            .Include(s => s.SnapshotPage)
            .Where(s => s.MediaId == mediaId && s.SnapshotPage!.TenantId == tenantId)
            .ToListAsync();

        return snapshots.ConvertAll(s => new MediaUsageDetail
        {
            EntityType = "Snapshot",
            EntityId = s.Id,
            Title = s.SnapshotPage?.Title ?? $"Snapshot #{s.Id}",
            RoutePath = $"/{s.SnapshotPage!.Slug}#{s.SnapshotPage.PublishedVersionId}-{s.Id}"
        });
    }
}