namespace PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;

public class PublishedSnapshotPageDto
{
    public string Version { get; set; } = "1.0.0";
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public List<PublishedSnapshotSectionDto> Sections { get; set; } = [];
}
public class PublishedSnapshotSectionDto
{
    public string ComponentSelector { get; set; } = "";
    public string ContentJson { get; set; } = "{}";
    public int Order { get; set; }
    public List<PublishedSnapshotSectionDto> SubSections { get; set; } = [];
}
