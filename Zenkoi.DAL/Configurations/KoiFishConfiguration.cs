using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class KoiFishConfiguration : IEntityTypeConfiguration<KoiFish>
    {
        public void Configure(EntityTypeBuilder<KoiFish> builder)
        {
            builder.ToTable("KoiFishes");
            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseIdentityColumn();

            builder.Property(k => k.PondId)
                .IsRequired();

            builder.Property(k => k.BreedingProcessId);

            builder.Property(k => k.VarietyId)
                .IsRequired();

            builder.Property(k => k.RFID)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(k => k.Size)
                .HasColumnType("decimal(8,2)");

            builder.Property(k => k.BirthDate);

            builder.Property(k => k.Gender)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(k => k.HealthStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(k => k.ImagesVideos)
                .HasMaxLength(1000);

            builder.Property(k => k.SellingPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(k => k.BodyShape)
                .HasMaxLength(100);

            builder.Property(k => k.Description)
                .HasMaxLength(1000);

            builder.Property(k => k.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(k => k.UpdatedAt);

            builder.HasOne(k => k.Pond)
                .WithMany(p => p.KoiFishes)
                .HasForeignKey(k => k.PondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(k => k.BreedingProcess)
                .WithMany(bp => bp.KoiFishes)
                .HasForeignKey(k => k.BreedingProcessId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(k => k.Variety)
                .WithMany(v => v.KoiFishes)
                .HasForeignKey(k => k.VarietyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(k => k.KoiIncidents)
                .WithOne(ki => ki.KoiFish)
                .HasForeignKey(ki => ki.KoiFishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(k => k.OrderDetails)
                .WithOne(od => od.KoiFish)
                .HasForeignKey(od => od.KoiFishId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(k => k.RFID).IsUnique();
            builder.HasIndex(k => k.PondId);
            builder.HasIndex(k => k.VarietyId);
        }
    }
}
