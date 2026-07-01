namespace PortfolioSaaS.Domain.Common;

public class PagedList<T>
{
    public List<T> List { get; set; } = [];
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int MaxPageSize { get; set; }
}