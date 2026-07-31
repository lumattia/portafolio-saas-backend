using Amazon.S3;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PortfolioSaaS.Application.Mapping;
using PortfolioSaaS.Infrastructure.Configuration;
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
        services.AddScoped(typeof(BaseRepository<>));
        services.AddScoped(typeof(BaseRepository<>));


        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Configure R2 settings
        services.Configure<R2Settings>(configuration.GetSection(R2Settings.SectionName));
        services.AddScoped(sp => sp.GetRequiredService<IOptions<R2Settings>>().Value);

        // Register S3 client for Cloudflare R2
        var r2Settings = configuration.GetSection(R2Settings.SectionName).Get<R2Settings>();
        if (r2Settings != null && !string.IsNullOrEmpty(r2Settings.Endpoint))
        {
            var config = new AmazonS3Config
            {
                ServiceURL = r2Settings.Endpoint,
                ForcePathStyle = true,
                UseHttp = false
            };
            
            var credentials = new Amazon.Runtime.BasicAWSCredentials(
                r2Settings.AccessKeyId, 
                r2Settings.SecretAccessKey);
            
            services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, config));
        }

        services.AddScoped<FileStorageService>();
        services.AddScoped<UnitOfWork>();
        return services;
    }
}
