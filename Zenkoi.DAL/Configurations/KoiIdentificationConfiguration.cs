using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class KoiIdentificationConfiguration : IEntityTypeConfiguration<KoiIdentification>
    {
        public void Configure(EntityTypeBuilder<KoiIdentification> builder)
        {
            builder.ToTable("KoiIdentification");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseIdentityColumn();

            builder.Property(k => k.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(k => k.IdentifiedAs)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(k => k.Confidence)
                .HasPrecision(5, 2); 

            builder.Property(k => k.Distance)
                .HasPrecision(10, 6);

            builder.Property(k => k.TopPredictions)
                .HasMaxLength(4000);

            builder.Property(k => k.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(k => k.KoiFish)
                .WithMany()
                .HasForeignKey(k => k.KoiFishId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(k => k.CreatedByUser)
                .WithMany()
                .HasForeignKey(k => k.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(k => k.CreatedAt);
            builder.HasIndex(k => k.KoiFishId);
            builder.HasIndex(k => k.IsUnknown);
        }
    }
}
