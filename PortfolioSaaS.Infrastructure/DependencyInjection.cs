using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioSaaS.Application.Mapping;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AuthProfile>();
            cfg.AddMaps(Assembly.GetAssembly(typeof(RendererProfile)));
        });

        services.AddScoped<TenantContext>();
        services.AddScoped<JwtTokenService>();
        services.AddTransient(typeof(PagedListConverter<,>));
        // Register repositories
        services.AddScoped(typeof(TenantBaseRepository<>));
        services.AddScoped(typeof(BaseRepository<>));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
