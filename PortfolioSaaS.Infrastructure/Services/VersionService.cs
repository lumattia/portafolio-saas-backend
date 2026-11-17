using AutoMapper;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Application.DTOs.Snapshots;
using PortfolioSaaS.Application.DTOs.Versions;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class VersionService(
    BaseRepository<PageSnapshot> pageSnapshotRepository,
    BaseRepository<PublishedVersion> versionRepository,
    TenantContext tenantContext,
    IMapper mapper)
{
    private readonly BaseRepository<PageSnapshot> _pageSnapshotRepository = pageSnapshotRepository;
    private readonly BaseRepository<PublishedVersion> _versionRepository = versionRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<VersionListDto> GetVersionsAsync()
    {
        var versions = await _versionRepository.GetAllAsIdNameAsync(v => new IdName(v.Id, $"{v.Number} — {v.Name}"));

        return new VersionListDto
        {
            CurrentVersionId = _tenantContext.CurrentVersion?.Id,
            Versions = versions
        };
    }

    public async Task<PagedList<SnapshotPageDto>> GetPagesAsync(Guid versionId, PagedParameters parameters)
    {
        var snapshots = await _pageSnapshotRepository.PageBySpecAsync(
            SnapshotSpecs.PageSnapshotsByVersion(parameters, versionId));
        return _mapper.Map<PagedList<SnapshotPageDto>>(snapshots);
    }
}
