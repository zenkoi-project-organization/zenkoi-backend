using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.ValidFrom)
                .IsRequired();

            builder.Property(p => p.ValidTo)
                .IsRequired();

            builder.Property(p => p.DiscountType)
                    .IsRequired();

            builder.Property(p => p.DiscountValue)
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
            builder.Property(p => p.MinimumOrderAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0); 

            builder.Property(p => p.MaxDiscountAmount)
                   .HasColumnType("decimal(18,2)"); 

            builder.Property(p => p.UsageCount)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(p => p.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(p => p.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(p => p.Images)
                   .HasMaxLength(1000);

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasIndex(p => p.Code).IsUnique();
            builder.HasIndex(p => p.ValidFrom);
            builder.HasIndex(p => p.ValidTo);
        }
    }
}
