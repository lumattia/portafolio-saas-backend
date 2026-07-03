using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Infrastructure.Services;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Data;

public class TenantBaseRepository<T>(ApplicationDbContext db, TenantContext tenantContext) where T : class, ITenantEntity
{
    private readonly ApplicationDbContext _db = db;
    private readonly TenantContext _tenantContext = tenantContext;

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
        EnsureTenantId(entity);
        var set = _db.Set<T>();
        var tracked = await GetByIdAsync(entity.Id, cancellationToken);

        if (tracked is null)
        {
            set.Add(entity);
        }
        else
        {
            AssertBelongsToCurrentTenant(entity);
            _db.Entry(tracked).CurrentValues.SetValues(entity);
        }
        return entity;
    }
    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdOrThrowAsync(id, cancellationToken);
        await DeleteAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        AssertBelongsToCurrentTenant(entity);
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            AssertBelongsToCurrentTenant(entity);
        _db.Set<T>().RemoveRange(entities);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
        => BuildQuery().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    public async Task<T> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException($"Entity not found with ID: {id} or not belonging to current tenant.");
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
            specification.PagedParameters.PageIndex = 1;
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
        var tenantId = GetCurrentTenantId();
        return _db.Set<T>().Where(x => x.TenantId == tenantId);
    }

    private Guid GetCurrentTenantId()
    {
        if (!_tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant context is not resolved.");

        return _tenantContext.CurrentTenantId!.Value;
    }

    private void EnsureTenantId(T entity)
    {
        if (entity.TenantId == Guid.Empty)
            entity.TenantId = GetCurrentTenantId();
    }

    private void AssertBelongsToCurrentTenant(T entity)
    {
        if (entity.TenantId != GetCurrentTenantId())
            throw new InvalidOperationException("Cannot operate on entity from different tenant.");
    }
    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(BuildQuery(), spec);
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
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.RollbackTransactionAsync(cancellationToken);
    }
}
