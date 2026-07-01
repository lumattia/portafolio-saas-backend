using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class SectionTemplateConfiguration : IEntityTypeConfiguration<SectionTemplate>
{
    public void Configure(EntityTypeBuilder<SectionTemplate> builder)
    {
        builder.HasKey(st => st.Id);
        builder.Property(st => st.Name).HasMaxLength(100).IsRequired();
        builder.Property(st => st.CategoryTags).HasConversion<int>();
    }
}
