using System.Linq.Expressions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Data;

public class BaseRepository<T>(ApplicationDbContext db) where T : class
{
    private readonly ApplicationDbContext _db = db;

    public async Task<T>SaveAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Save(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<T>> SaveAllAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in entities)
            await Save(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return list;
    }
    private async Task<T> Save(T entity, CancellationToken cancellationToken = default)
    {
        var set = _db.Set<T>();
        var tracked = await GetByIdAsync(GetKeyValues(entity), cancellationToken);

        if (tracked is null)
        {
            set.Add(entity);
        }
        else
        {
            _db.Entry(tracked).CurrentValues.SetValues(entity);
        }
        return entity;
    }
    public async Task DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdOrThrowAsync(id, cancellationToken);
        await DeleteAsync(entity, cancellationToken);
  }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _db.Set<T>().RemoveRange(entities);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        => _db.Set<T>().FindAsync([id], cancellationToken).AsTask();

    public async Task<T> GetByIdOrThrowAsync(object id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Entity not found with ID: {id} or not belonging to current tenant.");
    }
    public async Task<T> GetUniqueBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var count = await query.CountAsync(cancellationToken);
        if (count != 1) throw new InvalidOperationException("Entity not found with specification or not belonging to current tenant.");
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        return entity!;
    }
    public Task<T?> FirstOrDefaultBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    public Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).CountAsync(cancellationToken);

    public async Task<PagedList<T>> PageBySpecAsync(PagedSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var totalCount = await CountAsync(specification, cancellationToken);

        var pageParams= specification.PagedParameters;
        var page = pageParams.PageIndex;
        if (totalCount == 0 || (page - 1) * pageParams.PageSize >= totalCount)
        {
            page = 1;
        }
        specification.ApplyPage();

        var list = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return new PagedList<T>
        {
            List = list,
            PageIndex = page,
            PageSize = pageParams.PageSize,
            MaxPageSize = (int)Math.Ceiling((double)totalCount / pageParams.PageSize)
        };
    }


    public Task<List<IdName>> GetAllAsIdNameAsync(
        Expression<Func<T, IdName>> selector,
        CancellationToken cancellationToken = default)
    {
        return BuildQuery()
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<T> BuildQuery()
    {
        return _db.Set<T>().AsQueryable();
    }

    private object[] GetKeyValues(T entity)
    {
        var entityType = _db.Model.FindEntityType(typeof(T))!;
        var key = entityType.FindPrimaryKey()!;

        return [.. key.Properties.Select(p => _db.Entry(entity).Property(p.Name).CurrentValue!)];
    }
    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(BuildQuery(), spec);
    }
}
