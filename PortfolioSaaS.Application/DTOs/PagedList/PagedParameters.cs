using System.ComponentModel.DataAnnotations;

namespace PortfolioSaaS.Application.DTOs.PagedList;

public class PagedParameters
{
    [Range(1, int.MaxValue, ErrorMessage = "Page index must be at least 1.")]
    public int PageIndex { get; set; } = 1;
    [Range(5, 20, ErrorMessage = "PageSize too small or too big.")]
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = false;
}