using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            builder.HasKey(ci => ci.Id);
            
            builder.Property(ci => ci.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();
                
            builder.Property(ci => ci.CartId)
                .HasColumnName("CartId")
                .IsRequired();
                
            builder.Property(ci => ci.KoiFishId)
                .HasColumnName("KoiFishId");
                
            builder.Property(ci => ci.PacketFishId)
                .HasColumnName("PacketFishId");
                
            builder.Property(ci => ci.Quantity)
                .HasColumnName("Quantity")
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(ci => ci.UnitPrice)
                .HasColumnName("UnitPrice")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(ci => ci.AddedAt)
                .HasColumnName("AddedAt")
                .IsRequired();
                
            builder.Property(ci => ci.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
            builder.HasIndex(ci => new { ci.CartId, ci.KoiFishId })
                .IsUnique()
                .HasFilter("[KoiFishId] IS NOT NULL");

            builder.HasIndex(ci => new { ci.CartId, ci.PacketFishId })
                .IsUnique()
                .HasFilter("[PacketFishId] IS NOT NULL");

            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(ci => ci.KoiFish)
                .WithMany()
                .HasForeignKey(ci => ci.KoiFishId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(ci => ci.PacketFish)
                .WithMany()
                .HasForeignKey(ci => ci.PacketFishId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

