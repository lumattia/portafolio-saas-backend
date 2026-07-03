using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.ConfiguredDomain).IsUnique();
        builder.Property(t => t.ConfiguredDomain).HasMaxLength(256).IsRequired();

        builder.HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<User>(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.CurrentVersion)
            .WithMany()
            .HasForeignKey(u => u.CurrentVersionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
