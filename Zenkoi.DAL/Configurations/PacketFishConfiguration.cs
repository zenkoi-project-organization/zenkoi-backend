using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PacketFishConfiguration : IEntityTypeConfiguration<PacketFish>
    {
        public void Configure(EntityTypeBuilder<PacketFish> builder)
        {
            builder.ToTable("PacketFishes");
            builder.HasKey(pf => pf.Id);
            builder.Property(pf => pf.Id).UseIdentityColumn();

            builder.Property(pf => pf.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pf => pf.Description)
                .HasMaxLength(1000);

            builder.Property(pf => pf.FishPerPacket)
                .IsRequired()
                .HasDefaultValue(10);

            builder.Property(pf => pf.PricePerPacket)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(pf => pf.AgeMonths)
                .IsRequired()
                .HasColumnType("decimal(8,2)");

            builder.Property(pf => pf.Images)
                .HasMaxLength(1000);
            
            builder.Property(pf => pf.Videos)
                .HasMaxLength(1000);

            builder.Property(pf => pf.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pf => pf.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pf => pf.UpdatedAt);

            builder.HasMany(pf => pf.VarietyPacketFishes)
                .WithOne(vpf => vpf.PacketFish)
                .HasForeignKey(vpf => vpf.PacketFishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pf => pf.PondPacketFishes)
                .WithOne(ppf => ppf.PacketFish)
                .HasForeignKey(ppf => ppf.PacketFishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pf => pf.OrderDetails)
                .WithOne(od => od.PacketFish)
                .HasForeignKey(od => od.PacketFishId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pf => pf.Name);
        }
    }
}
