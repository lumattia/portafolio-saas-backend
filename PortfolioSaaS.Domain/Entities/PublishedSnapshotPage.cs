using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class PublishedSnapshotPage : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OriginalPageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Tenant Tenant { get; set; } = null!;
    public ICollection<PublishedSnapshotSection> Sections { get; set; } = [];
}
