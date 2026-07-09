using Microsoft.EntityFrameworkCore;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Services;

namespace PortfolioSaaS.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, TenantContext _tenantContext) : DbContext(options)
{
    public bool AutoSaveEnabled { get; set; } = true;
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<SectionTemplate> SectionTemplates => Set<SectionTemplate>();
    public DbSet<ThemeConfig> ThemeConfigs => Set<ThemeConfig>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<PageSnapshot> PublishedSnapshotPages => Set<PageSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(ITenantEntity.TenantId));
                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(Tenant))
                    .WithMany()
                    .HasForeignKey(nameof(ITenantEntity.TenantId))
                    .OnDelete(DeleteBehavior.Cascade);
                ConfigureTenantFilter(modelBuilder, entityType.ClrType);
            }
            if (typeof(ISnapshot).IsAssignableFrom(entityType.ClrType))
            {
                if (entityType.ClrType != typeof(MenuSnapshot))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex([
                            nameof(ISnapshot.TenantId),
                            nameof(ISnapshot.PublishedVersionId)
                        ])
                        .IsUnique();
                }
                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(nameof(ISnapshot.PublishedVersion))
                    .WithMany()
                    .HasForeignKey(nameof(ISnapshot.PublishedVersionId))
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
    private void ConfigureTenantFilter(ModelBuilder modelBuilder, Type entityType)
    {
        // Esto invoca el método SetTenantFilter<TEntity> de forma genérica
        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .MakeGenericMethod(entityType);

        method.Invoke(this, new object[] { modelBuilder });
    }

    private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenantEntity
    {
        // Aquí ocurre la magia: EF Core inyectará el filtro automáticamente
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _tenantContext.CurrentTenantId);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!AutoSaveEnabled) return 0;
        return await base.SaveChangesAsync(cancellationToken);
    }
}
