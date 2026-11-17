using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class SiteController(PublishService publishService) : ControllerBase
{
    private readonly PublishService _publishService = publishService;

    [HttpPost("publish")]
    public async Task<ActionResult<bool>> Publish([FromQuery] bool newVersion = false)
    {
        var result = await _publishService.PublishAsync(newVersion);
        return Ok(result);
    }
}
