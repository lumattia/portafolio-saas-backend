using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class ThemeConfig : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Color Light { get; set; } = new();
    public Color Dark { get; set; } = new();

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}
