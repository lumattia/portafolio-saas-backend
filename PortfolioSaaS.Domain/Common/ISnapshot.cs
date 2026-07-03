using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Domain.Common;

public interface ISnapshot: ITenantEntity
{
    public Guid PublishedVersionId { get; set; }
    public PublishedVersion PublishedVersion { get; set; }
}