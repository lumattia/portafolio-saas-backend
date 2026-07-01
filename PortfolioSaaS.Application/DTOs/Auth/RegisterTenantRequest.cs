namespace PortfolioSaaS.Application.DTOs.Auth;

public class RegisterTenantRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfiguredDomain { get; set; } = string.Empty;
}
