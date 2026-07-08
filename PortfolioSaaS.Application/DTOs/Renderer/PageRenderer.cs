using System.Text.Json;

namespace PortfolioSaaS.Application.DTOs.Renderer;

public class PageRenderer
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }

    public List<SectionRenderer> Sections { get; set; } = [];
}
public class SectionRenderer
{
    public Guid Id { get; set; }
    public string ComponentSelector { get; set; } = "";
    public JsonDocument ContentJson { get; set; } = JsonDocument.Parse("{}");
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
    public Guid? ParentSectionId { get; set; }
    public List<SectionRenderer> SubSections { get; set; } = [];
}