using AutoMapper;
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
    BaseRepository<Tenant> tenantRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<PageSnapshot> _pageSnapshotRepository = pageSnapshotRepository;
    private readonly TenantBaseRepository<MenuSnapshot> _menuSnapshotRepository = menuSnapshotRepository;
    private readonly TenantBaseRepository<ThemeConfigSnapshot> _themeConfigSnapshotRepository = themeConfigSnapshotRepository;
    private readonly TenantBaseRepository<PublishedVersion> _versionRepository = versionRepository;
    private readonly TenantBaseRepository<Page> _pageRepository = pageRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PageSnapshotDto?> GetPage(string slug, int? version)
    {
        var snapshotPage = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotsSpecs.GetPage(slug, _tenantContext.IsAuthenticated, version ?? _tenantContext.CurrentVersion?.Number));

        if (snapshotPage == null) return null;

        return _mapper.Map<PageSnapshotDto>(snapshotPage);
    }
    public async Task<List<MenuSnapshotDto>> GetAllMenus(int? version)
    {
        var menus = await _menuSnapshotRepository.GetAll(
            SnapshotsSpecs.GetMenuSnapshots(version ?? _tenantContext.CurrentVersion?.Number));

        return _mapper.Map<List<MenuSnapshotDto>>(menus);
    }
    public async Task<bool> PublishAsync(bool newVersion)
    {
       if (!_tenantContext.IsAuthenticated)
        return false;
        await _versionRepository.BeginTransactionAsync();
        try
        {
            var version = await _versionRepository.FirstOrDefaultBySpecAsync(PublishedVersionSpecs.GetLatestVersion());
            if (newVersion)
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
        var pages = await _pageRepository.GetAll(PageSpecs.ToPublish());
         foreach (var page in pages)
    {
        var snapshot = await _pageSnapshotRepository.FirstOrDefaultBySpecAsync(
            SnapshotsSpecs.GetPage(versionId, page.Id));

        if (snapshot == null)
        {
            snapshot = new PageSnapshot
            {
                Id = Guid.NewGuid(),
                PublishedVersionId = versionId,
                OriginalPageId = page.Id
            };
        }

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
    private void CreateSnapshotSections(List<Section> sections, Guid snapshotPageId, List<SectionSnapshot> snapshotSections)
    {
       for (int i = sections.Count - 1; i >= 0; i--)
        {
            var section = sections[i];

            if (section.IsDeleted)
            {
                sections.RemoveAt(i);
                continue;
            }
            var snapshotSection = new SectionSnapshot
            {
                Id = section.Id,
                SnapshotPageId = snapshotPageId,
                OriginalSectionId = section.Id,
                SectionTemplateId = section.SectionTemplateId,
                ContentJson = section.ContentJson,
                Order = section.Order,
                IsEnabled = section.IsEnabled,
                ParentSectionId = section.ParentSectionId,
            };
            snapshotSections.Add(snapshotSection);

            section.IsPublished = true;
        }
    }
    private async Task UpdateTenantVersion()
    {
        var tenant = await tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeVersion(_tenantContext.CurrentTenant!.Id));
        tenant!.CurrentVersionId = null;
        await _tenantRepository.SaveAsync(tenant);
    }
}
