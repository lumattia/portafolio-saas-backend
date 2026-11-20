namespace PortfolioSaaS.Domain.Common;

public class PagedList<T>
{
    public List<T> List { get; set; } = [];
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public static PagedList<T> EmptyList(int size) => new()
    {
        PageIndex = 1,
        PageSize = size,
        TotalPages = 0
    };
}