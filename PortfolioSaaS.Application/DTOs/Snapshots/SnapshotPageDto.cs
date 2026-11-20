namespace PortfolioSaaS.Application.DTOs.Snapshots;

public class SnapshotPageDto
{
    public Guid Id { get; set; }
    public Guid OriginalPageId { get; set; }
    public Guid PublishedVersionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public int VersionNumber { get; set; }
}
