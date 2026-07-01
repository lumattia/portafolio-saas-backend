using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Tenant)
            .WithOne(t => t.User)
            .HasForeignKey<User>(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
