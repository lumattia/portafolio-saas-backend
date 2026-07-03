using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
public class MenuController(MenuService menuService) : ControllerBase
{
    private readonly MenuService _menuService = menuService;
    [HttpGet]
    public async Task<ActionResult<List<MenuDto>>> GetMenu()
    {
        var menus = await _menuService.GetAllAsync();
        return Ok(menus);
    }
    [HttpPost]
    public async Task<ActionResult<MenuDto>> Create(MenuRequest request)
    {
        var menuItem = await _menuService.CreateAsync(request);
        if (menuItem == null)
            return Unauthorized();
        return Ok(menuItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MenuDto>> Update(Guid id, MenuRequest request)
    {
        var menuItem = await _menuService.UpdateAsync(id, request);
        if (menuItem == null)
            return NotFound();
        return Ok(menuItem);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _menuService.DeleteAsync(id);
        if (!result)
            return NotFound();
        return Ok();
    }
}
