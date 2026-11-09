using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    // Constructor without parameters for design time
    public ApplicationDbContextFactory() { }

    // Constructor with TenantContext for runtime
    public ApplicationDbContextFactory(TenantContext _tenantContext) { }

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        // For design time, create a dummy TenantContext
        var tenantContext = new TenantContext();
        return new ApplicationDbContext(optionsBuilder.Options, tenantContext);
    }
}
