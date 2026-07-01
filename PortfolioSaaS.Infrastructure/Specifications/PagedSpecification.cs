using System.Linq.Expressions;
using Ardalis.Specification;
using PortfolioSaaS.Application.DTOs.PagedList;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class PagedSpecification<T>(PagedParameters pagedParameters) : Specification<T>
{
    public PagedParameters PagedParameters { get; set; } = pagedParameters;
    public Expression<Func<T, object?>>? OrderExpression { get; set; }

    public void ApplyPage()
    {
        ApplyOrder();
        Query.Skip((PagedParameters.PageIndex - 1) * PagedParameters.PageSize).Take(PagedParameters.PageSize);
    }
    private void ApplyOrder()
    {
        if (OrderExpression == null) return;
        if (PagedParameters.IsDescending)
        {
            Query.OrderByDescending(OrderExpression);
        }
        else
        {
            Query.OrderBy(OrderExpression);
        }
    }
}
