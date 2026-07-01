using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.ContentJson).HasColumnType("jsonb").IsRequired();
        builder.Property(s => s.SnapshotSectionId).IsRequired(false);
        builder.HasOne(s => s.Page).WithMany(p => p.Sections).HasForeignKey(s => s.PageId);
        builder.HasOne(s => s.SectionTemplate).WithMany().HasForeignKey(s => s.SectionTemplateId);

        // Self-referencing relationship for hierarchical sections
        builder.HasOne(s => s.ParentSection)
            .WithMany(s => s.SubSections)
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
