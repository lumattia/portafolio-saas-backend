using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class SiteController(PublishingService publishingService) : ControllerBase
{
    private readonly PublishingService _publishingService = publishingService;

    [HttpPost("publish")]
    public async Task<ActionResult<bool>> Publish([FromQuery] bool newVersion = false)
    {
        var result = await _publishingService.PublishAsync(newVersion);
        return Ok(result);
    }
}
