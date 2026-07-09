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
        return _db.Set<T>()
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(_db.Set<T>(), spec);
    }
        public async Task BeginTransactionAsync()
    {
        await _db.Database.BeginTransactionAsync();
        _db.AutoSaveEnabled = false;
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        _db.AutoSaveEnabled = true;
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Database.CommitTransactionAsync(cancellationToken);
    }
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _db.Database.RollbackTransactionAsync(cancellationToken);
    }
}
