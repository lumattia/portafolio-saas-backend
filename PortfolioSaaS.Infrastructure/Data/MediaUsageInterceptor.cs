using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PortfolioSaaS.Domain.Entities;
using System.Reflection;

namespace PortfolioSaaS.Infrastructure.Data;

public class MediaUsageInterceptor : SaveChangesInterceptor
{
    // KNOWN LIMITATIONS:
    // 1. Race Conditions: If multiple users modify the same MediaId concurrently,
    //    they could read the same UsageCount and increment it separately, losing one increment.
    //    Mitigation: Currently not an issue as each tenant has only one user.
    //    Future fix: Use row-level locks or atomic SQL increments (UPDATE Media SET UsageCount = UsageCount + 1 WHERE Id = @id)
    //
    // 2. Mass Operations: ExecuteUpdate/ExecuteDelete bypass this interceptor.
    //    Example: _context.Sections.Where(s => s.MediaId == id).ExecuteUpdateAsync(...)
    //    Mitigation: Not currently used in codebase. If needed, manually update UsageCount after mass operations.
    //
    // 3. Transactional Consistency: If SaveChanges fails after modifying UsageCount,
    //    the transaction will rollback automatically, so no inconsistency occurs.

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        await TrackMediaUsageChangesAsync(context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChanges(eventData, result);

        TrackMediaUsageChangesAsync(context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    private async Task TrackMediaUsageChangesAsync(DbContext context, CancellationToken cancellationToken)
    {
        // Dictionary to accumulate deltas (+1 or -1) per MediaId to avoid duplicate queries
        var mediaDeltas = new Dictionary<Guid, int>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                var entityType = entry.Entity.GetType();
                var mediaProperties = GetMediaProperties(entityType);

                foreach (var prop in mediaProperties)
                {
                    var oldValue = entry.State == EntityState.Added ? null : entry.OriginalValues[prop.Name];
                    var newValue = entry.State == EntityState.Deleted ? null : entry.CurrentValues[prop.Name];

                    if (!Equals(oldValue, newValue))
                    {
                        if (oldValue is Guid oldMediaId)
                        {
                            mediaDeltas[oldMediaId] = mediaDeltas.GetValueOrDefault(oldMediaId, 0) - 1;
                        }

                        if (newValue is Guid newMediaId)
                        {
                            mediaDeltas[newMediaId] = mediaDeltas.GetValueOrDefault(newMediaId, 0) + 1;
                        }
                    }
                }
            }
        }

        // Apply grouped changes to database
        foreach (var (mediaId, delta) in mediaDeltas)
        {
            if (delta == 0) continue;

            var media = await context.Set<Media>().FindAsync([mediaId], cancellationToken);
            media?.UsageCount = Math.Max(0, media.UsageCount + delta);
        }
    }

    private static readonly Dictionary<Type, List<PropertyInfo>> _mediaPropertiesCache = [];
    private static readonly Lock _cacheLock = new();

    private List<PropertyInfo> GetMediaProperties(Type entityType)
    {
        lock (_cacheLock)
        {
            if (_mediaPropertiesCache.TryGetValue(entityType, out var cached))
                return cached;

            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(Guid?) && p.Name.Contains("Media", StringComparison.OrdinalIgnoreCase))
                .ToList();

            _mediaPropertiesCache[entityType] = properties;
            return properties;
        }
    }
}
