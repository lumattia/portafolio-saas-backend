using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.HasMany(s => s.SubMenuItems)
            .WithOne()
            .HasForeignKey(s => s.ParentMenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
