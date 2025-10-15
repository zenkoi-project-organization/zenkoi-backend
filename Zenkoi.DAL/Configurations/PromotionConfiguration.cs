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

            builder.Property(p => p.DiscountAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DiscountPercent)
                .HasColumnType("decimal(5,2)");

            builder.HasIndex(p => p.Code).IsUnique();
            builder.HasIndex(p => p.ValidFrom);
            builder.HasIndex(p => p.ValidTo);
        }
    }
}
