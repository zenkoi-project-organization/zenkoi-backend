using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PondTypeConfiguration : IEntityTypeConfiguration<PondType>
    {
        public void Configure(EntityTypeBuilder<PondType> builder)
        {
            builder.ToTable("PondTypes");
            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.Id).UseIdentityColumn();

            builder.Property(pt => pt.TypeName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pt => pt.RecommendedQuantity);

            builder.HasMany(pt => pt.Ponds)
                .WithOne(p => p.PondType)
                .HasForeignKey(p => p.PondTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pt => pt.WaterParameterThresholds)
                .WithOne(wpt => wpt.PondType)
                .HasForeignKey(wpt => wpt.PondTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pt => pt.TypeName).IsUnique();
        }
    }
}
