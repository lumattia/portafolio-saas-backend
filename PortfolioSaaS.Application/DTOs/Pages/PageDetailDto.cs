using System.Text.Json;

namespace PortfolioSaaS.Application.DTOs.Pages;

public class PageDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public bool IsDeleted { get; set; }
    public List<SectionDto> Sections { get; set; } = [];
}
public class SectionDto
{
    public Guid Id { get; set; }
    public Guid SectionTemplateId { get; set; }
    public string ComponentSelector { get; set; } = "";
    public JsonDocument ContentJson { get; set; } = JsonDocument.Parse("{}");
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsPublished { get; set; }
    // Hierarchical structure
    public Guid? ParentSectionId { get; set; }
    public List<SectionDto> SubSections { get; set; } = [];
}