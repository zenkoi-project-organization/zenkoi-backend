using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class WaterParameterRecordConfiguration : IEntityTypeConfiguration<WaterParameterRecord>
    {
        public void Configure(EntityTypeBuilder<WaterParameterRecord> builder)
        {
            builder.ToTable("WaterParameterRecords");
            builder.HasKey(wpr => wpr.Id);
            builder.Property(wpr => wpr.Id).UseIdentityColumn();

            builder.Property(wpr => wpr.PondId)
                .IsRequired();

            builder.Property(wpr => wpr.PHLevel)
                .HasColumnType("decimal(5,2)");

            builder.Property(wpr => wpr.TemperatureCelsius)
                .HasColumnType("decimal(5,2)");

            builder.Property(wpr => wpr.OxygenLevel)
                .HasColumnType("decimal(8,2)");

            builder.Property(wpr => wpr.AmmoniaLevel)
                .HasColumnType("decimal(8,4)");

            builder.Property(wpr => wpr.NitriteLevel)
                .HasColumnType("decimal(8,4)");

            builder.Property(wpr => wpr.NitrateLevel)
                .HasColumnType("decimal(8,4)");

            builder.Property(wpr => wpr.Turbidity)
                .HasColumnType("decimal(8,2)");

            builder.Property(wpr => wpr.TotalChlorines)
                .HasColumnType("decimal(8,4)");

            builder.Property(wpr => wpr.CarbonHardness)
                .HasColumnType("decimal(8,2)");

            builder.Property(wpr => wpr.WaterLevelMeters)
                .HasColumnType("decimal(8,2)");

            builder.Property(wpr => wpr.RecordedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wpr => wpr.RecordedByUserId);

            builder.Property(wpr => wpr.Notes)
                .HasMaxLength(1000);

            builder.HasOne(wpr => wpr.Pond)
                .WithMany(p => p.WaterParameters)
                .HasForeignKey(wpr => wpr.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wpr => wpr.RecordedBy)
                .WithMany()
                .HasForeignKey(wpr => wpr.RecordedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(wpr => wpr.PondId);
            builder.HasIndex(wpr => wpr.RecordedAt);
        }
    }
}
