using Ardalis.Specification;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PageService(
    TenantBaseRepository<Page> pageRepository,
    BaseRepository<SectionTemplate> templateRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<Page> _pageRepository = pageRepository;
    private readonly BaseRepository<SectionTemplate> _templateRepository = templateRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PageDetailDto?> GetByIdentifier(string identifier)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;
        var spec = PageSpecs.GetByIdentifierIncludeSection(identifier);
        var page = await _pageRepository.FirstOrDefaultBySpecAsync(spec);

        if (page == null) return null;

        return _mapper.Map<PageDetailDto>(page);
    }

    public async Task<PageDto?> CreateAsync(PageRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        if (tenantId == null) return null;

        var page = new Page
        {
            Id = request.Id ?? Guid.NewGuid(),
            TenantId = tenantId.Value,
            Title = request.Title,
            Slug = request.Slug,
            MetaDescription = request.MetaDescription,
            Disabled = request.Disabled
        };

        await _pageRepository.SaveAsync(page);

        return _mapper.Map<PageDto>(page);
    }

    public async Task<PageDetailDto?> UpdateWithSectionsAsync(string slug, PageRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection(slug));

        page.Title = request.Title;
        page.Slug = request.Slug;
        page.MetaDescription = request.MetaDescription;
        page.Disabled = request.Disabled;
        HashSet<Guid> templatesIds = [];
        // Sync sections recursively
        if (request.Sections != null)
        {
            await SyncSectionsRecursive(page.Sections, request.Sections, page.Id, null, templatesIds);
        }
        await _pageRepository.SaveAsync(page);
        if (templatesIds.Count > 0)
        {
            await Task.WhenAll(templatesIds.Select(id => _templateRepository.GetByIdAsync(id)));
        }
        return _mapper.Map<PageDetailDto>(page);
    }

    private async Task SyncSectionsRecursive(
        ICollection<Section> existingSections,
        List<SectionDto> incomingSections,
        Guid pageId,
        Guid? parentSectionId,
        HashSet<Guid> templatesIds)
    {
        var activeIncomingSections=incomingSections.Where(s => !s.IsDeleted);
        // Get IDs of incoming sections
        var activeIncomingIds = activeIncomingSections.Select(s => s.Id).ToHashSet();

        // Remove sections that are no longer in the incoming list
        var toRemove = existingSections
            .Where(s => !activeIncomingIds.Contains(s.Id))
            .ToList();
        foreach (var section in toRemove)
        {
            var hasSnapshot = section.SnapshotSectionId != null;
            if (hasSnapshot)
            {
                section.IsDeleted = true;
            }
            else
            {
                existingSections.Remove(section);
            }
        }

        // Update or create sections
        foreach (var sectionDto in activeIncomingSections)
        {
            var existingSection = existingSections.FirstOrDefault(s => s.Id == sectionDto.Id);

            if (existingSection == null)
            {
                existingSection = new Section
                {
                    Id = sectionDto.Id,
                    PageId = pageId,
                    ParentSectionId = parentSectionId,
                    ContentJson = "{}",
                    SectionTemplateId = sectionDto.SectionTemplateId,
                    SubSections = []
                };
                existingSections.Add(existingSection);
            }

            existingSection.ContentJson = sectionDto.ContentJson.RootElement.GetRawText();
            existingSection.Order = sectionDto.Order;
            existingSection.IsEnabled = sectionDto.IsEnabled;
            existingSection.ParentSectionId = parentSectionId;
            existingSection.IsDeleted = sectionDto.IsDeleted;
            if (existingSection.SectionTemplate == null)
            {
                templatesIds.Add(sectionDto.SectionTemplateId);
            }
            // Recursively sync sub-sections
            await SyncSectionsRecursive(
                existingSection.SubSections,
                sectionDto.SubSections,
                pageId,
                existingSection.Id,
                templatesIds);
        }
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        if (!_tenantContext.IsAuthenticated)
            return false;

        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection(slug));

        // Check if page has a published snapshot
        var hasSnapshot = page.SnapshotPageId != null;

        if (hasSnapshot)
        {
            // Soft delete: mark IsDeleted = true
            page.IsDeleted = true;
            await _pageRepository.SaveAsync(page);
        }
        else
        {
            // Hard delete if no snapshot
            await _pageRepository.DeleteAsync(page);
        }
        return true;
    }

    public async Task<bool> RestoreAsync(string slug)
    {
        if (!_tenantContext.IsAuthenticated)
            return false;

        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetBySlug(slug));

        // Restore the page by setting IsDeleted to false
        page.IsDeleted = false;
        await _pageRepository.SaveAsync(page);

        return true;
    }
}
