using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
               .ValueGeneratedNever();

            builder.HasOne(c => c.ApplicationUser)
                .WithOne()
                .HasForeignKey<Customer>(c => c.Id)
                .HasPrincipalKey<ApplicationUser>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.ShippingAddress)
                .HasMaxLength(500);

            builder.Property(c => c.ContactNumber)
                .HasMaxLength(20);

            builder.Property(c => c.TotalOrders)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.TotalSpent)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
