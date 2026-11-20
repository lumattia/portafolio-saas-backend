namespace PortfolioSaaS.Application.DTOs.Pages;

public class PageFilters
{
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public bool? ToPublish { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? Disabled { get; set; }
    public bool Latest { get; set; } = false;
}