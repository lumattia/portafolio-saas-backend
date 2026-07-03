using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class ThemeConfigSnapshotConfiguration : IEntityTypeConfiguration<ThemeConfigSnapshot>
{
    public void Configure(EntityTypeBuilder<ThemeConfigSnapshot> builder)
    {
        builder.HasKey(ms => ms.Id);
        builder.OwnsOne(tc => tc.Light);
        builder.OwnsOne(tc => tc.Dark);
    }
}
