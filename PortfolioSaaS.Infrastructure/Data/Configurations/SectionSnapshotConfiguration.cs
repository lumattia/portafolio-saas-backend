using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Data.Configurations;

public class SectionSnapshotConfiguration : IEntityTypeConfiguration<SectionSnapshot>
{
    public void Configure(EntityTypeBuilder<SectionSnapshot> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.ContentJson).HasColumnType("jsonb").IsRequired();
        builder.HasOne(s => s.SectionTemplate).WithMany().HasForeignKey(s => s.SectionTemplateId);

        // Self-referencing relationship for hierarchical sections
        builder.HasMany(s => s.SubSections)
            .WithOne()
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
