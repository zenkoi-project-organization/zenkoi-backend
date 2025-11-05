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

            builder.Property(ppf => ppf.PondId).IsRequired();
            builder.Property(ppf => ppf.PacketFishId).IsRequired();
            builder.Property(ppf => ppf.BreedingProcessId).IsRequired();
            builder.Property(ppf => ppf.AvailableQuantity).IsRequired();
            builder.Property(ppf => ppf.SoldQuantity).IsRequired();
            builder.Property(ppf => ppf.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(ppf => ppf.CreatedAt).IsRequired();

            builder.HasOne(ppf => ppf.Pond)
                .WithMany(p => p.PondPacketFishes)
                .HasForeignKey(ppf => ppf.PondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ppf => ppf.PacketFish)
                .WithMany(pf => pf.PondPacketFishes)
                .HasForeignKey(ppf => ppf.PacketFishId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ppf => ppf.BreedingProcess)
                .WithMany(bp => bp.PondPacketFishes)
                .HasForeignKey(ppf => ppf.BreedingProcessId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ppf => ppf.TransferredFrom)
                .WithOne(ppf => ppf.TransferredTo)
                .HasForeignKey<PondPacketFish>(ppf => ppf.TransferredFromId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
