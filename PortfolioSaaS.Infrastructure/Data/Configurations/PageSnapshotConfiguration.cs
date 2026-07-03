using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class PageSnapshotConfiguration : IEntityTypeConfiguration<PageSnapshot>
{
    public void Configure(EntityTypeBuilder<PageSnapshot> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(200).IsRequired();
        builder.Property(p => p.MetaDescription).HasMaxLength(500);
        builder.HasMany(p => p.Sections).WithOne().HasForeignKey(s => s.SnapshotPageId).OnDelete(DeleteBehavior.Cascade);
    }
}
