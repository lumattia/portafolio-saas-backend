using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.ContentJson).HasColumnType("jsonb").IsRequired();
        builder.HasOne(s => s.SectionTemplate).WithMany().HasForeignKey(s => s.SectionTemplateId);

        builder.OwnsOne(s => s.File);

        // Self-referencing relationship for hierarchical sections
        builder.HasMany(s => s.SubSections)
            .WithOne()
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
