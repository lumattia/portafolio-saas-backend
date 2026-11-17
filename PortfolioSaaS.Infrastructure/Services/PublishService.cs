using AutoMapper;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PublishService(
    BaseRepository<PageSnapshot> pageSnapshotRepository,
    BaseRepository<MenuSnapshot> menuSnapshotRepository,
    BaseRepository<ThemeConfigSnapshot> themeConfigSnapshotRepository,
    BaseRepository<PublishedVersion> versionRepository,
    BaseRepository<Page> pageRepository,
    BaseRepository<Menu> menuRepository,
    BaseRepository<ThemeConfig> themeConfigRepository,
    BaseRepository<Tenant> tenantRepository,
    TenantContext tenantContext,
    IMapper mapper,
    UnitOfWork unitOfWork)
{
    private readonly BaseRepository<PageSnapshot> _pageSnapshotRepository = pageSnapshotRepository;
    private readonly BaseRepository<MenuSnapshot> _menuSnapshotRepository = menuSnapshotRepository;
    private readonly BaseRepository<ThemeConfigSnapshot> _themeConfigSnapshotRepository = themeConfigSnapshotRepository;
    private readonly BaseRepository<PublishedVersion> _versionRepository = versionRepository;
    private readonly BaseRepository<Page> _pageRepository = pageRepository;
    private readonly BaseRepository<Menu> _menuRepository = menuRepository;
    private readonly BaseRepository<ThemeConfig> _themeConfigRepository = themeConfigRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;
    private readonly UnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> PublishAsync(bool newVersion)
    {
       if (!_tenantContext.IsAuthenticated) return false;
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var version = await _versionRepository.FirstOrDefaultBySpecAsync(VersionSpecs.GetLatestVersion());
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
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw; // Re-throw to let caller handle the exception
        }
    }
    private async Task<bool> PublishPages(Guid versionId)
    {
        var pages = await _pageRepository.GetAll(PublishSpecs.PagesToPublish());
        foreach (var page in pages)
        {
            var snapshot = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
                PublishSpecs.PageSnapshot(versionId, page.Id));

            snapshot ??= new PageSnapshot
            {
                Id = Guid.NewGuid(),
                PublishedVersionId = versionId,
                OriginalPageId = page.Id,
                TenantId = _tenantContext.CurrentTenantId!.Value
            };

            snapshot.Title = page.Title;
            snapshot.Slug = page.Slug;
            snapshot.MetaDescription = page.MetaDescription;
            snapshot.Disabled = page.Disabled;
            snapshot.IsDeleted = page.IsDeleted;
            page.Sections = [.. page.Sections.Where(s => !s.IsDeleted)];
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
        var menus = await _menuRepository.GetAll(PublishSpecs.MenusToPublish());
        foreach (var menu in menus)
        {
            var snapshot = await _menuSnapshotRepository.FirstOrDefaultBySpecAsync(
                PublishSpecs.MenuSnapshot(versionId, menu.Id));

            snapshot ??= new MenuSnapshot
            {
                Id = Guid.NewGuid(),
                Type = menu.Type,
                PublishedVersionId = versionId,
                OriginalMenuId = menu.Id,
                TenantId = _tenantContext.CurrentTenantId!.Value
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
        var themeConfigs = await _themeConfigRepository.GetAll(PublishSpecs.ThemeConfigsToPublish());
        foreach (var themeConfig in themeConfigs)
        {
            var snapshot = await _themeConfigSnapshotRepository.FirstOrDefaultBySpecAsync(
                PublishSpecs.ThemeConfigSnapshot(versionId, themeConfig.Id));

            snapshot ??= new ThemeConfigSnapshot
                {
                    Id = Guid.NewGuid(),
                    PublishedVersionId = versionId,
                OriginalThemeConfigId = themeConfig.Id,
                TenantId = _tenantContext.CurrentTenantId!.Value
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
