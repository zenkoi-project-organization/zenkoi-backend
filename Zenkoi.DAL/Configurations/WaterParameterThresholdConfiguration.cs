using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class WaterParameterThresholdConfiguration : IEntityTypeConfiguration<WaterParameterThreshold>
    {
        public void Configure(EntityTypeBuilder<WaterParameterThreshold> builder)
        {
            builder.ToTable("WaterParameterThresholds");
            builder.HasKey(wpt => wpt.Id);
            builder.Property(wpt => wpt.Id).UseIdentityColumn();

            builder.Property(wpt => wpt.ParameterName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wpt => wpt.Unit)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(wpt => wpt.MinValue)
                .IsRequired()
                .HasColumnType("decimal(10,4)");

            builder.Property(wpt => wpt.MaxValue)
                .IsRequired()
                .HasColumnType("decimal(10,4)");

            builder.Property(wpt => wpt.Notes)
                .HasMaxLength(500);

            builder.Property(wpt => wpt.PondTypeId);

            builder.HasOne(wpt => wpt.PondType)
                .WithMany(pt => pt.WaterParameterThresholds)
                .HasForeignKey(wpt => wpt.PondTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(wpt => wpt.ParameterName);
            builder.HasIndex(wpt => wpt.PondTypeId);
        }
    }
}
