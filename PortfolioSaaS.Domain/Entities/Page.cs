using System.Collections.ObjectModel;
using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class Page : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
    public bool ToPublish { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = null!;
    public List<Section> Sections { get; set; } = [];
}
