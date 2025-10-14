using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.OrderId)
                .IsRequired();

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaidAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.Gateway)
                .HasMaxLength(50);

            // Legacy fields for backward compatibility
            builder.Property(p => p.UserId);
            builder.Property(p => p.PaymentInfo)
                .HasMaxLength(500);
            builder.Property(p => p.BankName)
                .HasMaxLength(200);
            builder.Property(p => p.IsDefault)
                .HasDefaultValue(false);

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(p => p.OrderId).IsUnique();
            builder.HasIndex(p => p.TransactionId);
        }
    }
}
