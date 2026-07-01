using AutoMapper;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class ThemeConfigService(TenantBaseRepository<ThemeConfig> themeConfigRepository, BaseRepository<Tenant> tenantRepository, TenantContext tenantContext, IMapper mapper)
{
    private readonly TenantBaseRepository<ThemeConfig> _themeConfigRepository = themeConfigRepository;
    private readonly BaseRepository<Tenant> _tenantRepository = tenantRepository;
    private readonly TenantContext _tenantContext = tenantContext;
    private readonly IMapper _mapper = mapper;

    public async Task<ThemeConfigDto?> GetAsync()
    {
        if (!_tenantContext.IsResolved)
            return null;

        var tenantId = _tenantContext.CurrentTenantId!.Value;
        var tenant = await _tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeTheme(tenantId));
        var config = tenant.ThemeConfig;

        if (config == null)
        {
            config = new ThemeConfig
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId
            };
            await _themeConfigRepository.SaveAsync(config);
        }

        return _mapper.Map<ThemeConfigDto>(config);
    }

    public async Task<ThemeConfigDto?> UpdateAsync(ThemeConfigDto request)
    {
        if (!_tenantContext.IsAuthenticated)
            return null;

        var tenantId = _tenantContext.CurrentTenantId!.Value;

        var tenant = await _tenantRepository.GetUniqueBySpecAsync(TenantSpecs.IncludeTheme(tenantId));
        var config = tenant.ThemeConfig;

        if (config == null)
        {
            config = new ThemeConfig
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId
            };
            await _themeConfigRepository.SaveAsync(config);
        }

        config.Light.PrimaryColor = request.Light.PrimaryColor;
        config.Light.SecondaryColor = request.Light.SecondaryColor;
        config.Light.BackgroundColor = request.Light.BackgroundColor;
        config.Light.SurfaceColor = request.Light.SurfaceColor;
        config.Light.TextColor = request.Light.TextColor;
        config.Light.TextSecondaryColor = request.Light.TextSecondaryColor;
        config.Light.FontFamily = request.Light.FontFamily;
        config.Light.BorderRadius = request.Light.BorderRadius;

        config.Dark.PrimaryColor = request.Dark.PrimaryColor;
        config.Dark.SecondaryColor = request.Dark.SecondaryColor;
        config.Dark.BackgroundColor = request.Dark.BackgroundColor;
        config.Dark.SurfaceColor = request.Dark.SurfaceColor;
        config.Dark.TextColor = request.Dark.TextColor;
        config.Dark.TextSecondaryColor = request.Dark.TextSecondaryColor;
        config.Dark.FontFamily = request.Dark.FontFamily;
        config.Dark.BorderRadius = request.Dark.BorderRadius;

        await _themeConfigRepository.SaveAsync(config);

        return _mapper.Map<ThemeConfigDto>(config);
    }
}
