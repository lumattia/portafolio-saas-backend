using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class MenuItem : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid? PageId { get; set; }
    public Page? Page { get; set; }
    public string? ExternalUrl { get; set; }
    public bool IsExternal { get; set; }
    public int Order { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}
