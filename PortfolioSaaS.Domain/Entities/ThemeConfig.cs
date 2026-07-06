using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class ThemeConfig : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool ToPublish { get; set; } = true;
    public Color Light { get; set; } = new();
    public Color Dark { get; set; } = new();
}
