using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PondPacketFishConfiguration : IEntityTypeConfiguration<PondPacketFish>
    {
        public void Configure(EntityTypeBuilder<PondPacketFish> builder)
        {
            builder.ToTable("PondPacketFishes");
            builder.HasKey(ppf => ppf.Id);
            builder.Property(ppf => ppf.Id).UseIdentityColumn();

            builder.Property(ppf => ppf.PondId)
                .IsRequired();

            builder.Property(ppf => ppf.PacketFishId)
                .IsRequired();

            builder.Property(ppf => ppf.Quantity)
                .IsRequired();

            builder.HasOne(ppf => ppf.Pond)
                .WithMany(p => p.PondPacketFishes)
                .HasForeignKey(ppf => ppf.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ppf => ppf.PacketFish)
                .WithMany(pf => pf.PondPacketFishes)
                .HasForeignKey(ppf => ppf.PacketFishId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint to prevent duplicate pond-packet fish combinations
            builder.HasIndex(ppf => new { ppf.PondId, ppf.PacketFishId }).IsUnique();
        }
    }
}
