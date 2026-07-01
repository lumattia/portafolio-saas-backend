using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class Section
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public Guid SectionTemplateId { get; set; }

    // Content (editable by TenantOwner)
    public string ContentJson { get; set; } = "{}";

    // Reference to published snapshot section (nullable)
    public Guid? SnapshotSectionId { get; set; }

    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }

    // Hierarchical structure
    public Guid? ParentSectionId { get; set; }

    // Navigation
    public Page Page { get; set; } = null!;
    public SectionTemplate SectionTemplate { get; set; } = null!;
    public Section? ParentSection { get; set; }
    public ICollection<Section> SubSections { get; set; } = [];
}
