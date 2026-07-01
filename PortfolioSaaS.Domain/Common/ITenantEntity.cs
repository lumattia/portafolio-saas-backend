namespace PortfolioSaaS.Domain.Common;

public interface ITenantEntity
{
    Guid Id { get; set; }
    Guid TenantId { get; set; }
}
