using AutoMapper;
using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Application.Mapping;

public class PagedListProfile : Profile
{
    public PagedListProfile()
    {
        CreateMap(typeof(PagedList<>), typeof(PagedList<>))
            .ConvertUsing(typeof(PagedListConverter<,>));
    }
}