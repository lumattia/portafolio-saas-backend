using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(mi => mi.Id);

        builder.HasOne(mi => mi.Tenant)
            .WithMany(t => t.MenuItems)
            .HasForeignKey(mi => mi.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mi => mi.Page)
            .WithMany()
            .HasForeignKey(mi => mi.PageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
