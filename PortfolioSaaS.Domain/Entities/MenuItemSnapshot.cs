using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class MenuItemSnapshot
{
    public Guid Id { get; set; }
    public Guid SnapshotMenuId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid? ParentMenuItemId { get; set; }
    public List<MenuItemSnapshot> SubMenuItems { get; set; } = [];
}
