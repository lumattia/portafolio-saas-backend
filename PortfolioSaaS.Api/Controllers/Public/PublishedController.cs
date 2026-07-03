using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Snapshots;
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
     [HttpGet("menu")]
    public async Task<ActionResult<List<MenuSnapshotDto>>> GetMenu()
    {
        var menus = await _publishingService.GetAllMenus(null);
        return Ok(menus);
    }
}
