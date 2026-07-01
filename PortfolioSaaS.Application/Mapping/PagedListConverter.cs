using AutoMapper;
using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Application.Mapping;

public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
{
    public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
    {
        return new PagedList<TDestination>
        {
            PageIndex = source.PageIndex,
            PageSize = source.PageSize,
            MaxPageSize = source.MaxPageSize,
            List = context.Mapper.Map<List<TDestination>>(source.List)
        };
    }
}