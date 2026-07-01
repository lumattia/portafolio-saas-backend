using System.Text.Json;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PublishingService(
    TenantBaseRepository<PublishedSnapshotPage> snapshotPageRepository,
    TenantBaseRepository<Page> pageRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<PublishedSnapshotPage> _snapshotPageRepository = snapshotPageRepository;
    private readonly TenantBaseRepository<Page> _pageRepository = pageRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    private void CreateSnapshotSectionsRecursive(List<Section> sections, Guid snapshotPageId, Guid? parentSnapshotSectionId = null, List<PublishedSnapshotSection> snapshotSections = null!)
    {
        snapshotSections ??= [];

        foreach (var section in sections)
        {
            if (section.IsDeleted)
            {
                sections.Remove(section);
                continue;
            }

            var snapshotSection = new PublishedSnapshotSection
            {
                Id = Guid.NewGuid(),
                SnapshotPageId = snapshotPageId,
                OriginalSectionId = section.Id,
                SectionTemplateId = section.SectionTemplateId,
                ContentJson = section.ContentJson,
                Order = section.Order,
                IsEnabled = section.IsEnabled,
                ParentSectionId = parentSnapshotSectionId,
            };
            snapshotSections.Add(snapshotSection);

            // Update the original section to reference the snapshot
            section.SnapshotSectionId = snapshotSection.Id;

            // Recursively create snapshot sub-sections
            if (section.SubSections.Count > 0)
            {
                CreateSnapshotSectionsRecursive([.. section.SubSections], snapshotPageId, snapshotSection.Id, snapshotSections);
            }
        }
    }

    public async Task<PublishedSnapshotPageDto?> PublishPageAsync(string slug)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId;
        var page = await _pageRepository.GetUniqueBySpecAsync(PageSpecs.GetByIdentifierIncludeSection(slug));

        // If page is marked for deletion, delete it and its snapshot
        if (page.IsDeleted)
        {
            var snapshotToDelete = await _snapshotPageRepository.FirstOrDefaultBySpecAsync(
                new PublishedSnapshotPageSpecs.ByOriginalPageId(page.Id));
            if (snapshotToDelete != null)
            {
                await _snapshotPageRepository.DeleteAsync(snapshotToDelete);
            }
            await _pageRepository.DeleteAsync(page);
            return null;
        }

        var publishedSections = page.Sections.Where(s => s.IsEnabled && !s.IsDeleted).ToList();

        // Delete existing snapshot for this page
        var existingSnapshotPage = await _snapshotPageRepository.FirstOrDefaultBySpecAsync(
            new PublishedSnapshotPageSpecs.ByOriginalPageId(page.Id));
        if (existingSnapshotPage != null)
        {
            await _snapshotPageRepository.DeleteAsync(existingSnapshotPage);
        }

        // Create new snapshot page
        var snapshotPage = new PublishedSnapshotPage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId ?? page.TenantId,
            OriginalPageId = page.Id,
            Title = page.Title,
            Slug = page.Slug,
            MetaDescription = page.MetaDescription,
            PublishedAt = DateTime.UtcNow,
        };
        page.SnapshotPageId = snapshotPage.Id;
        var snapshotSections = new List<PublishedSnapshotSection>();
        CreateSnapshotSectionsRecursive(publishedSections, snapshotPage.Id, null, snapshotSections);

        snapshotPage.Sections = snapshotSections;
        await _snapshotPageRepository.SaveAsync(snapshotPage);

        return _mapper.Map<PublishedSnapshotPageDto>(snapshotPage);
    }

    public async Task<PublishedSnapshotPageDto?> GetPublishedPageAsync(string slug)
    {
        var snapshotPage = await _snapshotPageRepository.FirstOrDefaultBySpecAsync(
            new PublishedSnapshotPageSpecs.BySlug(slug));

        if (snapshotPage == null) return null;

        // Build complete snapshot with sections
        var snapshotContent = new
        {
            snapshotPageId = snapshotPage.Id,
            page = new
            {
                id = snapshotPage.OriginalPageId,
                title = snapshotPage.Title,
                slug = snapshotPage.Slug,
                metaDescription = snapshotPage.MetaDescription,
                publishedAt = snapshotPage.PublishedAt
            },
            sections = snapshotPage.Sections.Select(s => new
            {
                id = s.Id,
                originalSectionId = s.OriginalSectionId,
                sectionTemplateId = s.SectionTemplateId,
                content = s.ContentJson,
                order = s.Order,
                isEnabled = s.IsEnabled,
                parentSectionId = s.ParentSectionId,
                subSections = BuildSectionsTree([.. s.SubSections])
            }).ToList()
        };

        return _mapper.Map<PublishedSnapshotPageDto>(snapshotPage);
    }

    private List<object> BuildSectionsTree(List<PublishedSnapshotSection> sections)
    {
        return [.. sections.Select(s => new
        {
            id = s.Id,
            originalSectionId = s.OriginalSectionId,
            sectionTemplateId = s.SectionTemplateId,
            content = s.ContentJson,
            order = s.Order,
            isEnabled = s.IsEnabled,
            parentSectionId = s.ParentSectionId,
            subSections = BuildSectionsTree([.. s.SubSections])
        })];
    }
}
