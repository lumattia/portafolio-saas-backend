using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class PageSnapshot : ISnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OriginalPageId { get; set; }
    public Guid PublishedVersionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public List<SectionSnapshot> Sections { get; set; } = [];
    public PublishedVersion PublishedVersion { get; set; } = null!;
}
