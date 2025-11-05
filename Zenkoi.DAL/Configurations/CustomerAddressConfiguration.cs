using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.ToTable("CustomerAddresses");
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.FullAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ca => ca.City)
                .HasMaxLength(100);

            builder.Property(ca => ca.District)
                .HasMaxLength(100);

            builder.Property(ca => ca.Ward)
                .HasMaxLength(100);

            builder.Property(ca => ca.StreetAddress)
                .HasMaxLength(250);

            builder.Property(ca => ca.Latitude)
                .HasColumnType("decimal(9,6)");

            builder.Property(ca => ca.Longitude)
                .HasColumnType("decimal(9,6)");

            builder.Property(ca => ca.DistanceFromFarmKm)
                .HasColumnType("decimal(10,2)");

            builder.Property(ca => ca.RecipientPhone)
                .HasMaxLength(20);

            builder.Property(ca => ca.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ca => ca.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ca => ca.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ca => ca.UpdatedAt);

            builder.HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAddresses)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ca => ca.Orders)
                .WithOne(o => o.CustomerAddress)
                .HasForeignKey(o => o.CustomerAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
