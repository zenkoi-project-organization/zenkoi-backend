using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class EggBatchConfiguration : IEntityTypeConfiguration<EggBatch>
    {
        public void Configure(EntityTypeBuilder<EggBatch> builder)
        {
            // Tên bảng
            builder.ToTable("EggBatches");

            // Khóa chính
            builder.HasKey(e => e.Id);

            // Identity
            builder.Property(e => e.Id)
                   .UseIdentityColumn();

            // Thuộc tính cơ bản
            builder.Property(e => e.BreedingProcessId)
                   .IsRequired();

            builder.Property(e => e.PondId)
                   .IsRequired();

            builder.Property(e => e.Quantity)
                   .IsRequired(false);

            builder.Property(e => e.FertilizationRate)
                   .HasColumnType("float")
                   .IsRequired(false);

            builder.Property(e => e.HatchingTime)
                   .IsRequired(false);

            builder.Property(e => e.SpawnDate)
                   .IsRequired(false);

            // Enum Status — Lưu dưới dạng int để tránh lỗi cast
            builder.Property(e => e.Status)
                   .HasConversion<int>()
                   .IsRequired();

            // ====== Quan hệ ======

            // 1 BreedingProcess ↔ 1 EggBatch
            builder.HasOne(e => e.BreedingProcess)
                   .WithOne(bp => bp.Batch)
                   .HasForeignKey<EggBatch>(e => e.BreedingProcessId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1 Pond ↔ N EggBatch
            builder.HasOne(e => e.Pond)
                   .WithMany(p => p.EggBatches)
                   .HasForeignKey(e => e.PondId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1 EggBatch ↔ N IncubationDailyRecord
            builder.HasMany(e => e.IncubationDailyRecords)
                   .WithOne(r => r.EggBatch)
                   .HasForeignKey(r => r.EggBatchId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
