using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Enums;

namespace PortfolioSaaS.Domain.Entities;

public class User : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
