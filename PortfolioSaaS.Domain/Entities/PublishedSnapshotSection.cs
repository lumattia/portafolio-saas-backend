namespace PortfolioSaaS.Domain.Entities;

public class PublishedSnapshotSection
{
    public Guid Id { get; set; }
    public Guid SnapshotPageId { get; set; }
    public Guid OriginalSectionId { get; set; }
    public Guid SectionTemplateId { get; set; }
    public string ContentJson { get; set; } = "{}";
    public int Order { get; set; }
    public bool IsEnabled { get; set; }

    // Hierarchical structure
    public Guid? ParentSectionId { get; set; }

    // Navigation
    public PublishedSnapshotPage SnapshotPage { get; set; } = null!;
    public SectionTemplate SectionTemplate { get; set; } = null!;
    public PublishedSnapshotSection? ParentSection { get; set; }
    public ICollection<PublishedSnapshotSection> SubSections { get; set; } = [];
}
