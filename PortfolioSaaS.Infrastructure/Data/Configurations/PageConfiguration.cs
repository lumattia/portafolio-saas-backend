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
        builder.HasMany(p => p.Sections).WithOne().HasForeignKey(s => s.PageId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(p => new { p.TenantId, p.Slug }).IsUnique();
    }
}
