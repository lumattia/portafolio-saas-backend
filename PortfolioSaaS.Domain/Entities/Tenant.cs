
namespace PortfolioSaaS.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string ConfiguredDomain { get; set; } = string.Empty;
    public Guid? CurrentVersionId { get; set; }

    // Navigation
    public User? User { get; set; }
    public PublishedVersion? CurrentVersion { get; set; }
}
