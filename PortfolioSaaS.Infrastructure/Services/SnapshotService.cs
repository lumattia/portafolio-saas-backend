using AutoMapper;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class SnapshotService(
    BaseRepository<PageSnapshot> pageSnapshotRepository,
    BaseRepository<MenuSnapshot> menuSnapshotRepository,
    BaseRepository<ThemeConfigSnapshot> themeConfigSnapshotRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly BaseRepository<PageSnapshot> _pageSnapshotRepository = pageSnapshotRepository;
    private readonly BaseRepository<MenuSnapshot> _menuSnapshotRepository = menuSnapshotRepository;
    private readonly BaseRepository<ThemeConfigSnapshot> _themeConfigSnapshotRepository = themeConfigSnapshotRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PageRenderer?> GetPage(string slug, int? version)
    {
        var snapshotPage = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotSpecs.PageSnapshot(slug, _tenantContext.IsAuthenticated, version ?? _tenantContext.CurrentVersion?.Number));

        if (snapshotPage == null) return null;

        return _mapper.Map<PageRenderer>(snapshotPage);
    }

    public async Task<MenuRenderer> GetMenu(MenuType type, int? version)
    {
        var menus = await _menuSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotSpecs.MenuSnapshot(type, version ?? _tenantContext.CurrentVersion?.Number));

        return _mapper.Map<MenuRenderer>(menus);
    }

    public async Task<ThemeConfigSnapshotDto> GetThemeConfig(int? version)
    {
        var themeConfig = await _themeConfigSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotSpecs.ThemeConfigSnapshot(version ?? _tenantContext.CurrentVersion?.Number));

        return _mapper.Map<ThemeConfigSnapshotDto>(themeConfig);
    }
}
