using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class MenuSnapshot : ISnapshot
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public MenuType Type { get; set; }
    public Guid MenuId { get; set; }
    public Guid PublishedVersionId { get; set; }
    // Navigation
    public string ContentJson { get; set; } = "[]";
    public PublishedVersion PublishedVersion { get; set; } = null!;
}
