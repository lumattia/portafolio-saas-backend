using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.Menus;

public class MenuRequest
{
    public Guid? Id { get; set; }
    public MenuType Type { get; set; }
    public List<MenuItemRequest> MenuItems { get; set; } = [];
}

public class MenuItemRequest
{
    public Guid? Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Guid? ParentMenuItemId { get; set; }
    public List<MenuItemRequest> SubMenuItems { get; set; } = [];
}
