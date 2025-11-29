using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class ShippingDistanceConfiguration : IEntityTypeConfiguration<ShippingDistance>
    {
        public void Configure(EntityTypeBuilder<ShippingDistance> builder)
        {
            builder.ToTable("ShippingDistances");
            builder.HasKey(sd => sd.Id);
            builder.Property(sd => sd.Id).UseIdentityColumn();

            builder.Property(sd => sd.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sd => sd.MinDistanceKm)
                .IsRequired();

            builder.Property(sd => sd.MaxDistanceKm)
                .IsRequired();

            builder.Property(sd => sd.PricePerKm)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sd => sd.BaseFee)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sd => sd.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(sd => sd.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(sd => sd.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sd => sd.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(sd => sd.Name);
            builder.HasIndex(sd => sd.IsDeleted);
            builder.HasIndex(sd => new { sd.MinDistanceKm, sd.MaxDistanceKm });
            builder.HasIndex(sd => new { sd.IsDeleted, sd.MinDistanceKm, sd.MaxDistanceKm });
        }
    }
}
