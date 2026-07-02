using AutoMapper;
using PortfolioSaaS.Application.DTOs.PublishedSnapshotPages;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class PublishingService(
    TenantBaseRepository<PublishedSnapshotPage> snapshotPageRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly TenantBaseRepository<PublishedSnapshotPage> _snapshotPageRepository = snapshotPageRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<PublishedSnapshotPageDto?> GetPublishedPageAsync(string slug)
    {
        var snapshotPage = await _snapshotPageRepository.FirstOrDefaultBySpecAsync(
            PublishedSnapshotPageSpecs.GetByIdentifierIncludeSection(slug, !_tenantContext.IsAuthenticated));

        if (snapshotPage == null) return null;

        return _mapper.Map<PublishedSnapshotPageDto>(snapshotPage);
    }
}
