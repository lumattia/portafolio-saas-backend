using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Application.DTOs.SectionTemplates;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin")]
public class SectionTemplatesController(SectionTemplateService _service) : ControllerBase
{

    [HttpGet]
    public Task<PagedList<SectionTemplateDto>> GetAll([FromQuery] SectionTemplateFilterRequest request, [FromQuery] PagedParameters parameters)
    {
        return _service.GetAllAsync(request, parameters);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SectionTemplateDto?>> GetById(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is not null ? Ok(dto) : NotFound();
    }
    [HttpGet("section-categories")]
    public IActionResult GetSectionCategories()
    {
        var categories = Enum.GetValues<SectionCategory>()
            .Where(c => c != SectionCategory.None)
            .Select(c => new
            {
                Id = (int)c,
                Name = c.ToString(),
            });

        return Ok(categories);
    }
}
