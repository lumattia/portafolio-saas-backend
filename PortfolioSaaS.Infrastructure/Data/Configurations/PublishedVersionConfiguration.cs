using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class PublishedVersionConfiguration : IEntityTypeConfiguration<PublishedVersion>
{
    public void Configure(EntityTypeBuilder<PublishedVersion> builder)
    {
        builder.HasKey(ms => ms.Id);
        builder.HasIndex(p => new { p.TenantId, p.Number }).IsUnique();
    }
}
