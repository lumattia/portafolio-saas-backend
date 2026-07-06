using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class ThemeConfigSnapshot : ISnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OriginalThemeConfigId { get; set; }
    public Guid PublishedVersionId { get; set; }
    public PublishedVersion PublishedVersion { get; set; } = null!;
    public Color Light { get; set; } = new();
    public Color Dark { get; set; } = new();
}
