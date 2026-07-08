using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Menus;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PublishingService(
    TenantBaseRepository<PageSnapshot> pageSnapshotRepository,
    TenantBaseRepository<MenuSnapshot> menuSnapshotRepository,
    TenantBaseRepository<ThemeConfigSnapshot> themeConfigSnapshotRepository,
    TenantBaseRepository<PublishedVersion> versionRepository,
    TenantBaseRepository<Page> pageRepository,
    TenantBaseRepository<Menu> menuRepository,
    TenantBaseRepository<ThemeConfig> themeConfigRepository,
    BaseRepository<Tenant> tenantRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<PageSnapshot> _pageSnapshotRepository = pageSnapshotRepository;
    private readonly TenantBaseRepository<MenuSnapshot> _menuSnapshotRepository = menuSnapshotRepository;
    private readonly TenantBaseRepository<ThemeConfigSnapshot> _themeConfigSnapshotRepository = themeConfigSnapshotRepository;
    private readonly TenantBaseRepository<PublishedVersion> _versionRepository = versionRepository;
    private readonly TenantBaseRepository<Page> _pageRepository = pageRepository;
    private readonly TenantBaseRepository<Menu> _menuRepository = menuRepository;
    private readonly TenantBaseRepository<ThemeConfig> _themeConfigRepository = themeConfigRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;
    private readonly JsonSerializerOptions CamelCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public async Task<PageRenderer?> GetPage(string slug, int? version)
    {
        var snapshotPage = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotsSpecs.GetPage(slug, _tenantContext.IsAuthenticated, version ?? _tenantContext.CurrentVersion?.Number));

        if (snapshotPage == null) return null;

        return _mapper.Map<PageRenderer>(snapshotPage);
    }
    public async Task<MenuRenderer> GetMenu(MenuType type, int? version)
    {
        var menus = await _menuSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotsSpecs.GetMenu(type, version ?? _tenantContext.CurrentVersion?.Number));

        return _mapper.Map<MenuRenderer>(menus);
    }
    public async Task<ThemeConfigSnapshotDto> GetThemeConfig(int? version)
    {
        var themeConfig = await _themeConfigSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotsSpecs.GetThemeConfig(version ?? _tenantContext.CurrentVersion?.Number));

        return _mapper.Map<ThemeConfigSnapshotDto>(themeConfig);
    }

    public async Task<bool> PublishAsync(bool newVersion)
    {
       if (!_tenantContext.IsAuthenticated)
        return false;
        await _versionRepository.BeginTransactionAsync();
        try
        {
            var version = await _versionRepository.FirstOrDefaultBySpecAsync(PublishedVersionSpecs.GetLatestVersion());
            if (version == null || newVersion)
            {
                var versionNumber = version?.Number + 1 ?? 1;
                version = new PublishedVersion()
                {
                    Id = Guid.NewGuid(),
                    Name = versionNumber.ToString(),
                    Number = versionNumber,
                    TenantId = _tenantContext.CurrentTenantId!.Value
                };
                await _versionRepository.SaveAsync(version);
            }

            await PublishPages(version.Id);
            await PublishMenu(version.Id);
            await PublishThemeConfig(version.Id);
            await UpdateTenantVersion();
            await _versionRepository.CommitTransactionAsync();
            return true;
        }
        catch (Exception e)
        {
            await _versionRepository.RollbackTransactionAsync();
            return false;
        }
    }
    private async Task<bool> PublishPages(Guid versionId)
    {
        var pages = await _pageRepository.GetAll(SnapshotsSpecs.PageToPublish());
        foreach (var page in pages)
        {
            var snapshot = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
                SnapshotsSpecs.GetPage(versionId, page.Id));

            snapshot ??= new PageSnapshot
            {
                Id = Guid.NewGuid(),
                PublishedVersionId = versionId,
                OriginalPageId = page.Id
            };

            snapshot.Title = page.Title;
            snapshot.Slug = page.Slug;
            snapshot.MetaDescription = page.MetaDescription;
            snapshot.Disabled = page.Disabled;
            snapshot.IsDeleted = page.IsDeleted;

            page.Sections = page.Sections.Where(s => !s.IsDeleted).ToList();
            page.Sections.ForEach(s=>s.IsPublished=true);
            snapshot.Sections = _mapper.Map<List<SectionSnapshot>>(page.Sections);
            await _pageSnapshotRepository.SaveAsync(snapshot);

            page.IsPublished = true;
            page.ToPublish = false;

            if (page.IsDeleted)
                await _pageRepository.DeleteAsync(page);
            else
                await _pageRepository.SaveAsync(page);
        }
        return true;
    }
    private async Task<bool> PublishMenu(Guid versionId)
    {
        var menus = await _menuRepository.GetAll(SnapshotsSpecs.MenuToPublish());
        foreach (var menu in menus)
        {
            var snapshot = await _menuSnapshotRepository.FirstOrDefaultBySpecAsync(
                SnapshotsSpecs.GetMenu(versionId, menu.Id));

            snapshot ??= new MenuSnapshot
            {
                Id = Guid.NewGuid(),
                Type = menu.Type,
                PublishedVersionId = versionId,
                OriginalMenuId = menu.Id,
            };
            snapshot.MenuItems = _mapper.Map<List<MenuItemSnapshot>>(menu.MenuItems);
            await _menuSnapshotRepository.SaveAsync(snapshot);

            menu.ToPublish = false;
            await _menuRepository.SaveAsync(menu);
        }
        return true;
    }
        private async Task<bool> PublishThemeConfig(Guid versionId)
    {
        var themeConfigs = await _themeConfigRepository.GetAll(SnapshotsSpecs.ThemeConfigToPublish());
        foreach (var themeConfig in themeConfigs)
        {
            var snapshot = await _themeConfigSnapshotRepository.FirstOrDefaultBySpecAsync(
                SnapshotsSpecs.GetThemeConfig(versionId, themeConfig.Id));

            snapshot ??= new ThemeConfigSnapshot
                {
                    Id = Guid.NewGuid(),
                    PublishedVersionId = versionId,
                    OriginalThemeConfigId = themeConfig.Id
            };
            snapshot.Light = themeConfig.Light;
            snapshot.Dark = themeConfig.Dark;
            await _themeConfigSnapshotRepository.SaveAsync(snapshot);

            themeConfig.ToPublish = false;
            await _themeConfigRepository.SaveAsync(themeConfig);
        }
        return true;
    }
    private async Task<bool> UpdateTenantVersion()
    {
        var tenant = await _tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeVersion(_tenantContext.CurrentTenant!.Id));
        tenant!.CurrentVersionId = null;
        await _tenantRepository.SaveAsync(tenant);
        return true;
    }
}
