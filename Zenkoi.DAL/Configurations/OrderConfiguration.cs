using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).UseIdentityColumn();

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.UpdatedAt)
                .IsRequired(false);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.Subtotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippingFee)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DiscountAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Note)
                .HasMaxLength(2000);

            builder.Property(o => o.PromotionId);

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Promotion)
                .WithMany()
                .HasForeignKey(o => o.PromotionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.OrderNumber).IsUnique();
            builder.HasIndex(o => o.CustomerId);
            builder.HasIndex(o => o.CreatedAt);
            builder.HasIndex(o => o.UpdatedAt);
            builder.HasIndex(o => new { o.Status, o.UpdatedAt }); 
        }
    }
}
