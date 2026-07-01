using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public class PublishedController(PublishingService publishingService) : ControllerBase
{
    private readonly PublishingService _publishingService = publishingService;

    [HttpGet]
    [HttpGet("{*slug}")]
    public async Task<ActionResult<PublishedSnapshotPageDto?>> Get(string? slug = null)
    {
        var snapshot = await _publishingService.GetPublishedPageAsync(slug ?? "");
        return snapshot is not null ? Ok(snapshot) : NotFound();
    }
}
