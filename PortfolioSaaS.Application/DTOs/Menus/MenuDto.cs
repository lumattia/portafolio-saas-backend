namespace PortfolioSaaS.Application.DTOs.Menus;

public class MenuDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid? PageId { get; set; }
    public string PageSlug { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }
    public bool IsExternal { get; set; }
    public int Order { get; set; }
}

public class MenuRequest
{
    public Guid? Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string PageSlug { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }
    public bool IsExternal { get; set; }
    public int Order { get; set; }
}
