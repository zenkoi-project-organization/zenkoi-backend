using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class KoiGalleryEnrollmentConfiguration : IEntityTypeConfiguration<KoiGalleryEnrollment>
    {
        public void Configure(EntityTypeBuilder<KoiGalleryEnrollment> builder)
        {
            builder.ToTable("KoiGalleryEnrollment");

            builder.HasKey(k => k.Id);

            builder.Property(k => k.FishIdInGallery)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(k => k.EnrolledAt)
                .IsRequired();

            builder.HasOne(k => k.KoiFish)
                .WithMany()
                .HasForeignKey(k => k.KoiFishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(k => k.EnrolledByUser)
                .WithMany()
                .HasForeignKey(k => k.EnrolledBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(k => new { k.KoiFishId, k.IsActive });
            builder.HasIndex(k => k.FishIdInGallery);
        }
    }
}
