using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class VarietyPacketFishConfiguration : IEntityTypeConfiguration<VarietyPacketFish>
    {
        public void Configure(EntityTypeBuilder<VarietyPacketFish> builder)
        {
            builder.ToTable("VarietyPacketFishes");
            builder.HasKey(vpf => vpf.Id);
            builder.Property(vpf => vpf.Id).UseIdentityColumn();

            builder.Property(vpf => vpf.VarietyId)
                .IsRequired();

            builder.Property(vpf => vpf.PacketFishId)
                .IsRequired();

            builder.HasOne(vpf => vpf.Variety)
                .WithMany(v => v.VarietyPacketFishes)
                .HasForeignKey(vpf => vpf.VarietyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vpf => vpf.PacketFish)
                .WithMany(pf => pf.VarietyPacketFishes)
                .HasForeignKey(vpf => vpf.PacketFishId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint to prevent duplicate variety-packet fish combinations
            builder.HasIndex(vpf => new { vpf.VarietyId, vpf.PacketFishId }).IsUnique();
        }
    }
}
