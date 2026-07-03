using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class PagesController(PageService _pageService, PublishingService publishingService) : ControllerBase
{

    [HttpGet("{*identifier}")]
    public async Task<ActionResult<PageDetailDto?>> GetByIdentifier(string? identifier)
    {
        var page = await _pageService.GetByIdentifier(identifier ?? "");
        return page;
    }

    [HttpPost]
    public async Task<ActionResult<PageDto>> Create(PageRequest request)
    {
        var page = await _pageService.CreateAsync(request);
        if (page == null) return Unauthorized();
        return CreatedAtAction(nameof(GetByIdentifier), new { identifier = page.Slug }, page);
    }

    [HttpPut("{identifier}")]
    public async Task<ActionResult<PageDetailDto?>> Update(string identifier, PageRequest request)
    {
        var page = await _pageService.UpdateWithSectionsAsync(identifier, request);
        return page is not null ? Ok(page) : NotFound();
    }

    [HttpDelete("{identifier}")]
    public async Task<ActionResult> Delete(string identifier)
    {
        var success = await _pageService.DeleteAsync(identifier);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("publish")]
    public async Task<ActionResult<bool>> Publish()
    {
        var result = await publishingService.PublishAsync(true);
        return result;
    }

    [HttpPost("{identifier}/undo-delete")]
    public async Task<ActionResult> UndoDelete(string identifier)
    {
        // Restore the page by setting IsDeleted to false
        var success = await _pageService.RestoreAsync(identifier);
        if (!success) return NotFound();

        return Ok();
    }
}
