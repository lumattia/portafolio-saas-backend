using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class ThemeConfigConfiguration : IEntityTypeConfiguration<ThemeConfig>
{
    public void Configure(EntityTypeBuilder<ThemeConfig> builder)
    {
        builder.HasKey(tc => tc.Id);
        builder.HasOne(tc => tc.Tenant).WithOne(t => t.ThemeConfig).HasForeignKey<ThemeConfig>(tc => tc.TenantId);
        builder.OwnsOne(tc => tc.Light);
        builder.OwnsOne(tc => tc.Dark);
    }
}
