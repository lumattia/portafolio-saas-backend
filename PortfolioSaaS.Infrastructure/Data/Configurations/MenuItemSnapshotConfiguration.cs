using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class MenuItemSnapshotConfiguration : IEntityTypeConfiguration<MenuItemSnapshot>
{
    public void Configure(EntityTypeBuilder<MenuItemSnapshot> builder)
    {
        builder.HasKey(ms => ms.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.HasIndex(m => new { m.SnapshotMenuId, m.Id }).IsUnique();
    }
}
