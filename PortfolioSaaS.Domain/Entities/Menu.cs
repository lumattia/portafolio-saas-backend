using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;
public enum MenuType
{
    Sidebar,
}

public class Menu : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public MenuType Type { get; set; }
    public List<MenuItem> MenuItems { get; set; } = [];
}
