using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin")]
public class MediaController(MediaService mediaService) : ControllerBase
{
    private readonly MediaService _mediaService = mediaService;

    [HttpGet]
    public Task<PagedList<MediaDto>> GetAll([FromQuery] MediaFilterRequest request, [FromQuery] PagedParameters parameters)
    {
        return _mediaService.GetMediaAsync(request, parameters);
    }

    [HttpPost]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<MediaDto>> Create([FromForm] IFormFile file)
    {
        try
        {
            var result = await _mediaService.CreateMediaAsync(file);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MediaDto?>> Update(Guid id, [FromForm] IFormFile file)
    {
        try
        {
            var result = await _mediaService.UpdateMediaAsync(id, file);
            return result is not null ? Ok(result) : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _mediaService.DeleteMediaAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
