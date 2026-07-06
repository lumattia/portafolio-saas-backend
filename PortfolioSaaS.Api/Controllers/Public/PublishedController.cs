using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public class PublishedController(PublishingService publishingService) : ControllerBase
{
    private readonly PublishingService _publishingService = publishingService;

    [HttpGet]
    [HttpGet("{*slug}")]
    public async Task<ActionResult<PageSnapshotDto?>> Get(string? slug = null)
    {
        var snapshot = await _publishingService.GetPage(slug ?? "", null);
        return snapshot is not null ? Ok(snapshot) : NotFound();
    }
     [HttpGet("menu/{type}")]
    public async Task<ActionResult<MenuSnapshotDto>> GetMenu(MenuType type)
    {
        var menu = await _publishingService.GetMenu(type, null);
        return Ok(menu);
    }
    [HttpGet("theme")]
    public async Task<ActionResult<ThemeConfigSnapshotDto>> GetTheme()
    {
        var theme = await _publishingService.GetThemeConfig(null);
        return Ok(theme);
    }
}
