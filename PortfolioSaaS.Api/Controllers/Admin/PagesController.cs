using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class PagesController(PageService _pageService) : ControllerBase
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet("{*identifier}")]
    public async Task<ActionResult<PageRenderer?>> GetByIdentifier(string? identifier)
    {
        var page = await _pageService.GetByIdentifier(identifier ?? "");
        return page;
    }

    [HttpPost]
    public async Task<ActionResult<PageRenderer>> Create(PageRequest request)
    {
        var page = await _pageService.CreateAsync(request);
        if (page == null) return Unauthorized();
        return CreatedAtAction(nameof(GetByIdentifier), new { identifier = page.Slug }, page);
    }

    [HttpPut("{identifier}")]
    public async Task<ActionResult<PageRenderer?>> Update(string identifier, [FromForm] IFormCollection form)
    {
        var json = form["json"].ToString();
        var request = JsonSerializer.Deserialize<PageRequest>(json, _jsonOptions) ?? new PageRequest();
        var files = form.Files
                .Where(f => f.Name.StartsWith("file_"))
                .ToDictionary(f => f.Name, f => f);

        var page = await _pageService.UpdateWithSectionsAsync(identifier, request, files);
        return page is not null ? Ok(page) : NotFound();
    }

    [HttpDelete("{identifier}")]
    public async Task<ActionResult> Delete(string identifier)
    {
        var success = await _pageService.DeleteAsync(identifier);
        if (!success) return NotFound();
        return NoContent();
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
