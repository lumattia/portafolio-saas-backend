using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers;

[ApiController]
[Route("api/media")]
[Authorize]
public class MediaController(MediaService mediaService, TenantContext tenantContext) : ControllerBase
{
    private readonly MediaService _mediaService = mediaService;
    private readonly TenantContext _tenantContext = tenantContext;

    [HttpPost("upload")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResponse>> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved)
            return BadRequest(new { message = "Tenant context required." });

        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No image provided." });

        try
        {
            var relativePath = await _mediaService.SaveImageAsync(
                file,
                _tenantContext.CurrentTenantId!.Value,
                cancellationToken);

            var url = $"{Request.Scheme}://{Request.Host}{relativePath}";
            return Ok(new UploadImageResponse { Url = url });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
