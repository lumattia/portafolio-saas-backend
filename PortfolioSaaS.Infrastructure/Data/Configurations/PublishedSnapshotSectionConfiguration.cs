using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class PublishedSnapshotSectionConfiguration : IEntityTypeConfiguration<PublishedSnapshotSection>
{
    public void Configure(EntityTypeBuilder<PublishedSnapshotSection> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ContentJson).HasColumnType("jsonb").IsRequired();

        // Self-referencing relationship for hierarchical structure
        builder.HasOne(s => s.ParentSection)
            .WithMany(s => s.SubSections)
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.SectionTemplate)
            .WithMany()
            .HasForeignKey(s => s.SectionTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SnapshotPage)
            .WithMany(p => p.Sections)
            .HasForeignKey(s => s.SnapshotPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
