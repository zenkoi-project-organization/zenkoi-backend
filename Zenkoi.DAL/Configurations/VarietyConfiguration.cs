using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class VarietyConfiguration : IEntityTypeConfiguration<Variety>
    {
        public void Configure(EntityTypeBuilder<Variety> builder)
        {
            builder.ToTable("Varieties");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).UseIdentityColumn();

            builder.Property(v => v.VarietyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Characteristic)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(v => v.OriginCountry)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(v => v.KoiFishes)
                .WithOne(k => k.Variety)
                .HasForeignKey(k => k.VarietyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.VarietyPacketFishes)
                .WithOne(vpf => vpf.Variety)
                .HasForeignKey(vpf => vpf.VarietyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.VarietyName).IsUnique();
        }
    }
}
