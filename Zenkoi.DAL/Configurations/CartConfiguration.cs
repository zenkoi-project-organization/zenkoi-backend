using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
                
            builder.Property(c => c.CustomerId)
                .HasColumnName("CustomerId")
                .IsRequired();
                
            builder.Property(c => c.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();
                
            builder.Property(c => c.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
            
            // Relationships
            builder.HasOne(c => c.Customer)
                .WithOne(cu => cu.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

