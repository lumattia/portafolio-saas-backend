using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string ConfiguredDomain { get; set; } = string.Empty;

    // Navigation
    public User? User { get; set; }
    public ThemeConfig? ThemeConfig { get; set; }
    public List<MenuItem> MenuItems { get; set; } = [];
}
