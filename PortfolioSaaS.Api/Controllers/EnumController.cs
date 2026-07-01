using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnumController : ControllerBase
{
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
