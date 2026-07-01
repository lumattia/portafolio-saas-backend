using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Auth;
using PortfolioSaaS.Infrastructure.Services;
using PortfolioSaaS.Domain.Enums;

namespace PortfolioSaaS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(result);
    }

    [HttpPost("register-tenant")]
    [Authorize(Roles = nameof(UserRole.PlatformAdmin))]
    public async Task<ActionResult<LoginResponse>> RegisterTenant([FromBody] RegisterTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterTenantAsync(request, cancellationToken);
        if (result is null)
            return Conflict(new { message = "Email or domain already exists." });

        return CreatedAtAction(nameof(Login), result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirstValue("tenant_id");
        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        var success = await _authService.ChangePasswordAsync(tenantId, request, cancellationToken);
        if (!success)
            return BadRequest(new { message = "Invalid current password." });

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        return Ok(new
        {
            TenantId = User.FindFirstValue("tenant_id"),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name),
            Role = User.FindFirstValue(ClaimTypes.Role),
        });
    }
}
