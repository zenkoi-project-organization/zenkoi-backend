using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.OrderId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(p => p.PaymentUrl)
                .HasMaxLength(2048);

            builder.Property(p => p.ResponseData)
                .HasColumnType("NVARCHAR(MAX)");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(p => p.ActualOrder)
                .WithMany()
                .HasForeignKey(p => p.ActualOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.TransactionId);
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.ActualOrderId);
        }
    }
}
