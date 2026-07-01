using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Public;

[ApiController]
[Route("api/public/[controller]")]
public class MenuController(MenuService menuService) : ControllerBase
{
    private readonly MenuService _menuService = menuService;

    [HttpGet]
    public async Task<ActionResult<List<MenuDto>>> GetMenu()
    {
        var menus = await _menuService.GetAllAsync(true);
        return Ok(menus);
    }
}
