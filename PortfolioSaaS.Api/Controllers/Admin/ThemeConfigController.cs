using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class ThemeConfigController(ThemeConfigService themeConfigService) : ControllerBase
{
    private readonly ThemeConfigService _themeConfigService = themeConfigService;

    [HttpGet]
    public async Task<ActionResult<ThemeConfigDto?>> Get()
    {
        var dto = await _themeConfigService.GetAsync();
        return dto ?? new ThemeConfigDto();
    }

    [HttpPut]
    public async Task<ActionResult<ThemeConfigDto?>> Update(ThemeConfigDto request)
    {
        var dto = await _themeConfigService.UpdateAsync(request);
        return dto is not null ? Ok(dto) : Unauthorized();
    }
}
