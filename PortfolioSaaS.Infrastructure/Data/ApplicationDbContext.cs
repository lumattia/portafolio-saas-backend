using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<SectionTemplate> SectionTemplates => Set<SectionTemplate>();
    public DbSet<ThemeConfig> ThemeConfigs => Set<ThemeConfig>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<PublishedSnapshotPage> PublishedSnapshotPages => Set<PublishedSnapshotPage>();
    public DbSet<PublishedSnapshotSection> PublishedSnapshotSections => Set<PublishedSnapshotSection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure JSON columns
        modelBuilder.Entity<Section>()
            .Property(s => s.ContentJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<SectionTemplate>()
            .Property(st => st.DefaultContentJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Asset>()
            .Property(a => a.MetadataJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PublishedSnapshotSection>()
            .Property(s => s.ContentJson)
            .HasColumnType("jsonb");
    }
}
