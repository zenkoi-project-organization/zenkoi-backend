using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(od => od.Id);
            builder.Property(od => od.Id).UseIdentityColumn();

            builder.Property(od => od.OrderId)
                .IsRequired();

            builder.Property(od => od.KoiFishId);

            builder.Property(od => od.PacketFishId);

            builder.Property(od => od.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(od => od.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(od => od.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(od => od.KoiFish)
                .WithMany(k => k.OrderDetails)
                .HasForeignKey(od => od.KoiFishId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(od => od.PacketFish)
                .WithMany(pf => pf.OrderDetails)
                .HasForeignKey(od => od.PacketFishId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure either KoiFishId or PacketFishId is provided, but not both
            builder.HasCheckConstraint("CK_OrderDetail_KoiOrPacket",
                "(KoiFishId IS NOT NULL AND PacketFishId IS NULL) OR (KoiFishId IS NULL AND PacketFishId IS NOT NULL)");

            builder.HasIndex(od => od.OrderId);
        }
    }
}
