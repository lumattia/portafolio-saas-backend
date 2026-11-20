namespace PortfolioSaaS.Application.DTOs.Pages;

public class PageDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
    public bool ToPublish { get; set; }
}
