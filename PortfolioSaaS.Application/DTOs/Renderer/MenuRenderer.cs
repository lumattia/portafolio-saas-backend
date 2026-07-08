using System.Text.Json;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.Renderer;

public class MenuRenderer
{
    public Guid Id { get; set; }
    public MenuType Type { get; set; }
    // Navigation
    public List<MenuItemRenderer> MenuItems { get; set; } = [];
}
public class MenuItemRenderer
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Guid? ParentMenuItemId { get; set; }
    public List<MenuItemRenderer> SubMenuItems { get; set; } = [];
}