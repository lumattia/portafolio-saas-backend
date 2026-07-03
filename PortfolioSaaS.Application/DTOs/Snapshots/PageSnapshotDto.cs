using System.Text.Json;

namespace PortfolioSaaS.Application.DTOs.Snapshots;

public class PageSnapshotDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool Disabled { get; set; }
    public bool IsDeleted { get; set; }

    public List<SectionSnapshotDto> Sections { get; set; } = [];
}
public class SectionSnapshotDto
{
    public Guid Id { get; set; }
    public string ComponentSelector { get; set; } = "";
    public JsonDocument ContentJson { get; set; } = JsonDocument.Parse("{}");
    public List<SectionSnapshotDto> SubSections { get; set; } = [];
}
