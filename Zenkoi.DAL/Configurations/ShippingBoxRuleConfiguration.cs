using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class ShippingBoxRuleConfiguration : IEntityTypeConfiguration<ShippingBoxRule>
    {
        public void Configure(EntityTypeBuilder<ShippingBoxRule> builder)
        {
            builder.ToTable("ShippingBoxRules");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).UseIdentityColumn();

            builder.Property(r => r.ShippingBoxId)
                .IsRequired();

            builder.Property(r => r.RuleType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.MaxCount)
                .IsRequired(false);

            builder.Property(r => r.MaxLengthCm)
                .IsRequired(false);

            builder.Property(r => r.MinLengthCm)
                .IsRequired(false);

            builder.Property(r => r.MaxWeightLb)
                .IsRequired(false);

            builder.Property(r => r.ExtraInfo)
                .HasMaxLength(500);

            builder.Property(r => r.Priority)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.ShippingBox)
                .WithMany(sb => sb.Rules)
                .HasForeignKey(r => r.ShippingBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.ShippingBoxId);
            builder.HasIndex(r => new { r.ShippingBoxId, r.Priority });
            builder.HasIndex(r => r.IsActive);
        }
    }
}