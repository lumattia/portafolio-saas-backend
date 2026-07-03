using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class MenuItemSnapshotConfiguration : IEntityTypeConfiguration<MenuSnapshot>
{
    public void Configure(EntityTypeBuilder<MenuSnapshot> builder)
    {
        builder.HasKey(ms => ms.Id);
        builder.Property(ms => ms.ContentJson).HasColumnType("jsonb").IsRequired();
    }
}
