using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(200).IsRequired();
        builder.Property(p => p.MetaDescription).HasMaxLength(500);
        builder.HasIndex(p => new { p.TenantId, p.Slug }).IsUnique();
        builder.Property(p => p.SnapshotPageId).IsRequired(false);
        builder.HasOne(p => p.Tenant).WithMany().HasForeignKey(p => p.TenantId);
        builder.HasMany(p => p.Sections).WithOne(s => s.Page).HasForeignKey(s => s.PageId).OnDelete(DeleteBehavior.Cascade);
    }
}
