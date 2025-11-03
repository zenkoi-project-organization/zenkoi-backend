using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class ShippingBoxConfiguration : IEntityTypeConfiguration<ShippingBox>
    {
        public void Configure(EntityTypeBuilder<ShippingBox> builder)
        {
            builder.ToTable("ShippingBoxes");
            builder.HasKey(sb => sb.Id);
            builder.Property(sb => sb.Id).UseIdentityColumn();

            builder.Property(sb => sb.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sb => sb.WeightCapacityLb)
                .IsRequired();

            builder.Property(sb => sb.Fee)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sb => sb.MaxKoiCount)
                .IsRequired(false);

            builder.Property(sb => sb.MaxKoiSizeInch)
                .IsRequired(false);

            builder.Property(sb => sb.Notes)
                .HasMaxLength(500);

            builder.Property(sb => sb.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(sb => sb.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sb => sb.UpdatedAt)
                .IsRequired(false);

            builder.HasMany(sb => sb.Rules)
                .WithOne(r => r.ShippingBox)
                .HasForeignKey(r => r.ShippingBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sb => sb.Name);
            builder.HasIndex(sb => sb.IsActive);
        }
    }
}