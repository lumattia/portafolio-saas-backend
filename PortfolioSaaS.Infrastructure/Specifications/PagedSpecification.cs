using System.Linq.Expressions;
using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.PagedList;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class PagedSpecification<T>(PagedParameters pagedParameters) : Specification<T>
{
    public PagedParameters PagedParameters { get; set; } = pagedParameters;
    public Expression<Func<T, object?>>? OrderExpression { get; set; }
    public Func<IQueryable<T>, IQueryable<T>, IQueryable<T>>? QueryTransform { get; set; }
}
