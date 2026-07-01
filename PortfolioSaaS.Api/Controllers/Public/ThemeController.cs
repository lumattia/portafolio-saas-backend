using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public class ThemeController(ThemeConfigService themeConfigService) : ControllerBase
{
    private readonly ThemeConfigService _themeConfigService = themeConfigService;

    [HttpGet]
    public async Task<ActionResult> GetTheme()
    {
        var theme = await _themeConfigService.GetAsync();
        if (theme == null) return NotFound();
        return Ok(theme);
    }
}
