using Ardalis.Specification;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PortfolioSaaS.Application.DTOs.Pages;
using PortfolioSaaS.Application.DTOs.Renderer;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PageService(
    BaseRepository<Page> pageRepository,
    BaseRepository<SectionTemplate> templateRepository,
    TenantContext tenantContext,
    IMapper mapper,
    FileStorageService fileStorageService,
    UnitOfWork unitOfWork)
{
    private readonly BaseRepository<Page> _pageRepository = pageRepository;
    private readonly BaseRepository<SectionTemplate> _templateRepository = templateRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;
    private readonly FileStorageService _fileStorageService = fileStorageService;
    private readonly UnitOfWork _unitOfWork = unitOfWork;

    public async Task<PageRenderer?> GetByIdentifier(string identifier)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;
        var spec = PageSpecs.GetByIdentifierIncludeSection(identifier);
        var page = await _pageRepository.FirstOrDefaultBySpecAsync(spec);

        if (page == null) return null;

        return _mapper.Map<PageRenderer>(page);
    }

    public async Task<PageRenderer?> CreateAsync(PageRequest request)
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

        return _mapper.Map<PageRenderer>(page);
    }

    public async Task<PageRenderer?> UpdateWithSectionsAsync(string slug, PageRequest request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection(slug));

            page.Title = request.Title;
            page.Slug = request.Slug;
            page.MetaDescription = request.MetaDescription;
            page.Disabled = request.Disabled;
            page.ToPublish = true;
            HashSet<Guid> templatesIds = [];
            // Sync sections recursively
            if (request.Sections != null)
            {
                await SyncSections(page.Sections, request.Sections, page, templatesIds);
            }
            await _pageRepository.SaveAsync(page);

            await _unitOfWork.CommitAsync();

            // Loading missing templates
            if (templatesIds.Count > 0)
            {
                await Task.WhenAll(templatesIds.Select(id => _templateRepository.GetByIdAsync(id)));
            }
            return _mapper.Map<PageRenderer>(page);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task SyncSections(
        List<Section> existingSections,
        List<SectionRequest> incomingSections,
        Page page,
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
            var hasSnapshot = section.IsPublished;
            if (hasSnapshot)
            {
                section.IsDeleted = true;
            }
            else
            {
                // Delete files from storage when section is hard deleted                
                _fileStorageService.QueueDeleteLog(section.File);
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
                    Page = page,
                    PageId = page.Id,
                    ParentSectionId = sectionDto.ParentSectionId,
                    ContentJson = "{}",
                    SectionTemplateId = sectionDto.SectionTemplateId,
                    SubSections = []
                };
                existingSections.Add(existingSection);
            }

            existingSection.ContentJson = sectionDto.ContentJson.RootElement.GetRawText();
            existingSection.Order = sectionDto.Order;
            existingSection.IsEnabled = sectionDto.IsEnabled;
            existingSection.IsDeleted = sectionDto.IsDeleted;

            if (sectionDto.File != null)
            {
                existingSection.File = null;
                _fileStorageService.QueueDeleteLog(existingSection.File);
                if (sectionDto.File.Base64 != "")
                {
                    existingSection.File = _fileStorageService.QueueUpload(
                        sectionDto.File.FileName,
                        sectionDto.File.ContentType,
                        sectionDto.File.Base64,
                        existingSection.GetStoragePath());
                }
            }

            if (existingSection.SectionTemplate == null)
            {
                templatesIds.Add(sectionDto.SectionTemplateId);
            }
        }
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        if (!_tenantContext.IsAuthenticated)
            return false;

        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection(slug));

        // Check if page has a published snapshot
        var hasSnapshot = page.IsPublished;

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

        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection((slug)));

        // Restore the page by setting IsDeleted to false
        page.IsDeleted = false;
        await _pageRepository.SaveAsync(page);

        return true;
    }

   

}
