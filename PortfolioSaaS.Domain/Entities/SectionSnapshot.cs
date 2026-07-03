namespace PortfolioSaaS.Domain.Entities;

public class SectionSnapshot
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
    public SectionTemplate SectionTemplate { get; set; } = null!;
    public List<SectionSnapshot> SubSections { get; set; } = [];
}