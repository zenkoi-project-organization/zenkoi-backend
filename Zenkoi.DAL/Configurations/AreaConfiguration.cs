using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("Areas");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).UseIdentityColumn();

            builder.Property(a => a.AreaName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.TotalAreaSQM)
                .HasColumnType("decimal(10,2)");

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.HasMany(a => a.Ponds)
                .WithOne(p => p.Area)
                .HasForeignKey(p => p.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
