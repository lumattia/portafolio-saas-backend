using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;
using System.Reflection;

namespace PortfolioSaaS.Infrastructure.Services;

public class MediaUsageDetailsCalculator(
    ApplicationDbContext context,
    TenantContext tenantContext)
{
    private readonly ApplicationDbContext _context = context;
    private readonly TenantContext _tenantContext = tenantContext;
    private static readonly Dictionary<Type, List<PropertyInfo>> _mediaPropertiesCache = [];
    private static readonly Dictionary<Type, Type> _specificationCache = [];
    private static readonly Lock _cacheLock = new();

    public async Task<int> GetUsageCountAsync(Guid mediaId, CancellationToken cancellationToken = default)
    {
        var locations = await GetUsageDetailsAsync(mediaId, cancellationToken);
        return locations.Count;
    }

    public async Task<List<MediaUsageDetail>> GetUsageDetailsAsync(Guid mediaId, CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsResolved)
            return [];

        var tenantId = _tenantContext.CurrentTenantId!.Value;
        var allLocations = new List<MediaUsageDetail>();

        var mediaReferencingTypes = GetMediaReferencingEntityTypes();

        foreach (var entityType in mediaReferencingTypes)
        {
            try
            {
                var locations = await GetMediaLocationsAsync(entityType, mediaId, tenantId, cancellationToken);
                allLocations.AddRange(locations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting media locations in {entityType.Name}: {ex.Message}");
            }
        }

        return allLocations;
    }

    private static List<Type> GetMediaReferencingEntityTypes()
    {
        lock (_cacheLock)
        {
            if (_mediaPropertiesCache.Count > 0)
            {
                return [.. _mediaPropertiesCache.Keys];
            }

            var entityTypes = typeof(Page).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.StartsWith("PortfolioSaaS.Domain.Entities") is true)
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType == typeof(Guid?) && p.Name.Contains("Media", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (properties.Count > 0)
                {
                    _mediaPropertiesCache[entityType] = properties;
                }
            }

            return [.. _mediaPropertiesCache.Keys];
        }
    }

    private async Task<List<MediaUsageDetail>> GetMediaLocationsAsync(Type entityType, Guid mediaId, Guid tenantId, CancellationToken cancellationToken)
    {
        var specType = GetSpecificationType(entityType);
        if (specType == null)
            return [];

        var specInstance = Activator.CreateInstance(specType);
        var getSpecMethod = specType.GetMethod("GetUsageLocations");
        var result = getSpecMethod?.Invoke(specInstance, [mediaId, tenantId, _context]);

        if (result is Task<List<MediaUsageDetail>> task)
        {
            return await task;
        }

        return [];
    }

    private static Type GetSpecificationType(Type entityType)
    {
        lock (_cacheLock)
        {
            if (_specificationCache.TryGetValue(entityType, out var cached))
                return cached;

            var specInterfaceType = typeof(IMediaUsageSpecification<>).MakeGenericType(entityType);
            var specImplementation = typeof(MediaUsageDetailsCalculator).Assembly
                .GetTypes()
                .FirstOrDefault(t => specInterfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            _specificationCache[entityType] = specImplementation;
            return specImplementation;
        }
    }

    public static void InvalidateCache()
    {
        lock (_cacheLock)
        {
            _mediaPropertiesCache.Clear();
            _specificationCache.Clear();
        }
    }
}
