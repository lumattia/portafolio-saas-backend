using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Infrastructure.Data;

public interface IBaseRepository<T> where T : class
{
    Task<T> SaveAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(object id, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken =default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<T> GetByIdOrThrowAsync(object id, CancellationToken cancellationToken = default);
    Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultBySpecAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<PagedList<T>> PageBySpecAsync(ISpecification<T> specification, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<IdName>> GetAllAsIdNameAsync(Expression<Func<T, IdName>> selector, CancellationToken cancellationToken = default);
}