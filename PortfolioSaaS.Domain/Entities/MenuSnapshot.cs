using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class MenuSnapshot : ISnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public MenuType Type { get; set; }
    public Guid OriginalMenuId { get; set; }
    public Guid PublishedVersionId { get; set; }
    // Navigation
    public PublishedVersion PublishedVersion { get; set; } = null!;
    public List<MenuItemSnapshot> MenuItems { get; set; } = [];
}
