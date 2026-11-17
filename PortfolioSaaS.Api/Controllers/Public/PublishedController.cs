using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public class PublishedController(SnapshotService snapshotService) : ControllerBase
{
    private readonly SnapshotService _snapshotService = snapshotService;

    [HttpGet]
    [HttpGet("{*slug}")]
    public async Task<ActionResult<PageRenderer?>> Get(string? slug = null)
    {
        var snapshot = await _snapshotService.GetPage(slug ?? "", null);
        return snapshot is not null ? Ok(snapshot) : NotFound();
    }
     [HttpGet("menu/{type}")]
    public async Task<ActionResult<MenuRenderer>> GetMenu(MenuType type)
    {
        var menu = await _snapshotService.GetMenu(type, null);
        return Ok(menu);
    }
    [HttpGet("theme")]
    public async Task<ActionResult<ThemeConfigSnapshotDto>> GetTheme()
    {
        var theme = await _snapshotService.GetThemeConfig(null);
        return Ok(theme);
    }
}
