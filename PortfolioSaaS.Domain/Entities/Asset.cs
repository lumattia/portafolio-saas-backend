using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public enum AssetType
{
    Image,
    Document,
    Video
}

public class Asset : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public AssetType Type { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? MetadataJson { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}
