using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class WaterAlertConfiguration : IEntityTypeConfiguration<WaterAlert>
    {
        public void Configure(EntityTypeBuilder<WaterAlert> builder)
        {
            builder.ToTable("WaterAlerts");
            builder.HasKey(wa => wa.Id);
            builder.Property(wa => wa.Id).UseIdentityColumn();

            builder.Property(wa => wa.PondId)
                .IsRequired();

            builder.Property(wa => wa.ParameterName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wa => wa.MeasuredValue)
                .IsRequired()
                .HasColumnType("decimal(10,4)");

            builder.Property(wa => wa.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(wa => wa.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wa => wa.IsResolved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(wa => wa.ResolvedByUserId);

            builder.HasOne(wa => wa.Pond)
                .WithMany()
                .HasForeignKey(wa => wa.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wa => wa.ResolvedBy)
                .WithMany()
                .HasForeignKey(wa => wa.ResolvedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(wa => wa.PondId);
            builder.HasIndex(wa => wa.CreatedAt);
            builder.HasIndex(wa => wa.IsResolved);
        }
    }
}
