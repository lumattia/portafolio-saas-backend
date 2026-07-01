using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.SectionTemplates;
public class SectionTemplateDto
{
    public Guid Id { get; set; }
    public string ComponentSelector { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SectionCategory CategoryTags { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string DefaultContentJson { get; set; } = "{}";
}
public class SectionTemplateFilterRequest
{
    public string? Name { get; set; }
    public SectionCategory? CategoryTags { get; set; }
}
