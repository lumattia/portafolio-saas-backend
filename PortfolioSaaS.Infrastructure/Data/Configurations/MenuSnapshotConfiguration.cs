using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class MenuSnapshotConfiguration : IEntityTypeConfiguration<MenuSnapshot>
{
    public void Configure(EntityTypeBuilder<MenuSnapshot> builder)
    {
        builder.HasKey(ms => ms.Id);
        builder.HasIndex(m => new { m.TenantId, m.PublishedVersionId, m.Type }).IsUnique();
        builder.HasMany(mi => mi.MenuItems)
            .WithOne()
            .HasForeignKey(mi => mi.MenuSnashotId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(p => new { p.TenantId, p.Type, p.PublishedVersionId }).IsUnique();
    }
}