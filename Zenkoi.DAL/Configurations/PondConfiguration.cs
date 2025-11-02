using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PondConfiguration : IEntityTypeConfiguration<Pond>
    {
        public void Configure(EntityTypeBuilder<Pond> builder)
        {
            builder.ToTable("Ponds");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();

            builder.Property(p => p.PondTypeId)
                .IsRequired();

            builder.Property(p => p.AreaId)
                .IsRequired();

            builder.Property(p => p.PondName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Location)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.PondStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.CapacityLiters)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.DepthMeters)
                .HasColumnType("decimal(8,2)");

            builder.Property(p => p.LengthMeters)
                .HasColumnType("decimal(8,2)");

            builder.Property(p => p.WidthMeters)
                .HasColumnType("decimal(8,2)");

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.PondType)
                .WithMany(pt => pt.Ponds)
                .HasForeignKey(p => p.PondTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Area)
                .WithMany(a => a.Ponds)
                .HasForeignKey(p => p.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.KoiFishes)
                .WithOne(k => k.Pond)
                .HasForeignKey(k => k.PondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.WaterParameters)
                .WithOne(wpr => wpr.Pond)
                .HasForeignKey(wpr => wpr.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PondPacketFishes)
                .WithOne(ppf => ppf.Pond)
                .HasForeignKey(ppf => ppf.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PondIncidents)
                .WithOne(pi => pi.Pond)
                .HasForeignKey(pi => pi.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.PondTypeId);
            builder.HasIndex(p => p.AreaId);
            builder.HasIndex(p => p.PondName);
        }
    }
}
