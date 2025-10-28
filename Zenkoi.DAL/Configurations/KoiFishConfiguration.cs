using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Configurations
{
    public class KoiFishConfiguration : IEntityTypeConfiguration<KoiFish>
    {
        public void Configure(EntityTypeBuilder<KoiFish> builder)
        {
            builder.ToTable("KoiFishes");

            // Key & Identity
            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseIdentityColumn();

            // Foreign keys
            builder.Property(k => k.PondId)
                .IsRequired();

            builder.Property(k => k.BreedingProcessId);

            builder.Property(k => k.VarietyId)
                .IsRequired();

            // Basic properties
            builder.Property(k => k.RFID)
                .IsRequired()
                .HasMaxLength(50);

          
            builder.Property(k => k.Size)
                .IsRequired()
                .HasConversion<string>();  // FishSize enum

            builder.Property(k => k.Type)
                .IsRequired()
                .HasConversion<string>();  // KoiType enum

            builder.Property(k => k.Gender)
                .IsRequired()
                .HasConversion<string>();  

            builder.Property(k => k.HealthStatus)
                .IsRequired()
                .HasConversion<string>(); 

            builder.Property(k => k.SaleStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(SaleStatus.NotForSale);

            builder.Property(k => k.BirthDate);

            builder.Property(k => k.SellingPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(k => k.BodyShape)
                .HasMaxLength(100);

            builder.Property(k => k.Description)
                .HasMaxLength(1000);
            builder.Property(k => k.ColorPattern)
               .HasMaxLength(1000);
            builder.Property(k => k.Origin)
                .HasMaxLength(1000);

            builder.Property(k => k.Images)
             .HasMaxLength(1000);

            builder.Property(k => k.Videos)
         .HasMaxLength(1000);
            // Timestamps
            builder.Property(k => k.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(k => k.UpdatedAt);

            // Relationships
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
