using AutoMapper;
using PortfolioSaaS.Application.DTOs.Pages;
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
            Versions = [.. versions.OrderByDescending(v => int.TryParse(v.Name.Split('—')[0].Trim(), out var number) ? number : 0)]
        };
    }

    public async Task<PagedList<SnapshotPageDto>> GetPagesAsync(Guid? versionId, PagedParameters parameters, PageFilters filters)
    {
        int? versionNumber = null;
        if (versionId.HasValue)
        {
            versionNumber = (await _versionRepository.FirstOrDefaultBySpecAsync(VersionSpecs.ById(versionId.Value)))?.Number;
            if (versionNumber is null)
                return PagedList<SnapshotPageDto>.EmptyList(parameters.PageSize);
        }
        // Necesario? O con nulo tambien funciona, debería funcionar....Y si no está seleccionada, debería devolver la última si quiere la actual que la seleccione.
        else if (filters.Latest)
        {
            versionNumber = _tenantContext.CurrentVersion?.Number
                ?? (await _versionRepository.FirstOrDefaultBySpecAsync(VersionSpecs.GetLatestVersion()))?.Number;
            if (versionNumber is null)
                return PagedList<SnapshotPageDto>.EmptyList(parameters.PageSize);
        }

        PagedList<PageSnapshot> snapshots = await _pageSnapshotRepository.PageBySpecAsync(
            SnapshotSpecs.PageSnapshotsByVersion(parameters, versionNumber, filters));
        return _mapper.Map<PagedList<SnapshotPageDto>>(snapshots);
    }
}