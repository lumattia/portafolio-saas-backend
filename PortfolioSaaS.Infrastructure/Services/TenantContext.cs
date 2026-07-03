using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Domain.Enums;

namespace PortfolioSaaS.Infrastructure.Services;

public class TenantContext
{
    public Tenant? CurrentTenant { get; private set; }
    public User? CurrentUser { get; private set; }
    public Guid? CurrentTenantId => CurrentTenant?.Id;
    public UserRole? CurrentUserRole => CurrentUser?.Role;
    public bool IsAuthenticated => CurrentUser != null;
    public bool IsResolved => CurrentTenant != null;
    public bool IsPlatformAdmin => CurrentUserRole == UserRole.PlatformAdmin;
    public PublishedVersion? CurrentVersion => CurrentTenant?.CurrentVersion;

    public void SetTenant(Tenant currentTenant)
    {
        CurrentTenant = currentTenant;
    }
    public void SetUser(User? currentUser)
    {
        CurrentUser = currentUser;
    }
}
