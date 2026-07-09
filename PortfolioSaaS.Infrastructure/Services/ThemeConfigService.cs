using Ardalis.Specification;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class ThemeConfigService(BaseRepository<ThemeConfig> themeConfigRepository, BaseRepository<Tenant> tenantRepository, TenantContext tenantContext, IMapper mapper)
{
    private readonly BaseRepository<ThemeConfig> _themeConfigRepository = themeConfigRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<ThemeConfigDto?> GetAsync()
    {
        var themeConfig = await _themeConfigRepository.FirstOrDefaultBySpecAsync(new Specification<ThemeConfig>());
        if (themeConfig == null)
        {
            if (_tenantContext.IsResolved)
            {
                themeConfig = new ThemeConfig
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.CurrentTenantId!.Value
                };
                await _themeConfigRepository.SaveAsync(themeConfig);
            }
            else
            {
                return null;
            }
        }

        return _mapper.Map<ThemeConfigDto>(themeConfig);
    }

    public async Task<ThemeConfigDto?> UpdateAsync(ThemeConfigDto request)
    {
        if (!_tenantContext.IsResolved) return null;
        var themeConfig = await _themeConfigRepository.GetByIdAsync(request.Id);
        if (themeConfig == null)
        {
            themeConfig = new ThemeConfig
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.CurrentTenantId!.Value
            };
        }

        themeConfig.Light = request.Light;
        themeConfig.Dark = request.Dark;
        await _themeConfigRepository.SaveAsync(themeConfig);

        return _mapper.Map<ThemeConfigDto>(themeConfig);
    }
}
