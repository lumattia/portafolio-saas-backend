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
        var tracked = await GetByIdAsync(_db.Entry(entity).Property("Id").CurrentValue!, cancellationToken);

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
        return await GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Entity not found with ID: {id}.");
    }
    public async Task<T> GetUniqueBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var count = await query.CountAsync(cancellationToken);
        if (count != 1) throw new InvalidOperationException("Entity not found with specification or not belonging to current tenant.");
        return await query.FirstAsync(cancellationToken);
    }
    public Task<T?> FirstOrDefaultBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    public Task<int> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).CountAsync(cancellationToken);
    public Task<List<T>> GetAll(Specification<T> specification, CancellationToken cancellationToken = default)
            => ApplySpecification(specification).ToListAsync(cancellationToken);
    public async Task<PagedList<T>> PageBySpecAsync(PagedSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var pageParams = specification.PagedParameters;
        var query = ApplySpecification(specification);
        if (specification.QueryTransform is not null) query = specification.QueryTransform(query, _db.Set<T>());
        if (specification.OrderExpression is not null) query = pageParams.IsDescending ? query.OrderByDescending(specification.OrderExpression) : query.OrderBy(specification.OrderExpression);
        var totalCount = await query.CountAsync(cancellationToken);
        var page = pageParams.PageIndex;
        if (totalCount == 0 || (page - 1) * pageParams.PageSize >= totalCount) page = 1;
        var list = await query.Skip((page - 1) * pageParams.PageSize).Take(pageParams.PageSize).ToListAsync(cancellationToken);
        return new PagedList<T>
        {
            List = list,
            PageIndex = page,
            PageSize = pageParams.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageParams.PageSize)
        };
    }


    public Task<List<IdName>> GetAllAsIdNameAsync(
        Expression<Func<T, IdName>> selector,
        CancellationToken cancellationToken = default)
    {
        return _db.Set<T>()
            .Select(selector)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(_db.Set<T>(), spec);
    }
}
