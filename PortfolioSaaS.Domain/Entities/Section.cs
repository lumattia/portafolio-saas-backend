using System.Collections.ObjectModel;
using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class Section
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public Guid SectionTemplateId { get; set; }

    public string ContentJson { get; set; } = "{}";


    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
    // Hierarchical structure
    public Guid? ParentSectionId { get; set; }

    // Navigation
    public Page Page { get; set; } = null!;
    public SectionTemplate SectionTemplate { get; set; } = null!;
    public Section? ParentSection { get; set; }
    public List<Section> SubSections { get; set; } = [];
}
