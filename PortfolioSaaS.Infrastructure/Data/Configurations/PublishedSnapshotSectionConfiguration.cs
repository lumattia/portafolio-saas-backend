using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class PublishedSnapshotSectionConfiguration : IEntityTypeConfiguration<SectionSnapshot>
{
    public void Configure(EntityTypeBuilder<SectionSnapshot> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ContentJson).HasColumnType("jsonb").IsRequired();

        // Self-referencing relationship for hierarchical structure
          builder.HasMany(s => s.SubSections)
            .WithOne()
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.SectionTemplate)
            .WithMany()
            .HasForeignKey(s => s.SectionTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
