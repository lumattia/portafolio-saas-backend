using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Application.DTOs.Versions;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "PlatformAdmin,TenantOwner")]
public class VersionsController(VersionService versionService) : ControllerBase
{
    private readonly VersionService _versionService = versionService;

    [HttpGet]
    public Task<VersionListDto> GetVersions()
    {
        return _versionService.GetVersionsAsync();
    }
}
