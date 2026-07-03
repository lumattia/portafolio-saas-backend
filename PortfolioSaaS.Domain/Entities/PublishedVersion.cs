using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public class PublishedVersion: ITenantEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Number { get; set; }
    public Guid TenantId { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
