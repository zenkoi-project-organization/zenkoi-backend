using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class PondIncidentConfiguration : IEntityTypeConfiguration<PondIncident>
    {
        public void Configure(EntityTypeBuilder<PondIncident> builder)
        {
            builder.ToTable("PondIncidents");
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Id).UseIdentityColumn();

            builder.Property(pi => pi.IncidentId)
                .IsRequired();

            builder.Property(pi => pi.PondId)
                .IsRequired();

            builder.Property(pi => pi.EnvironmentalChanges)
                .HasMaxLength(2000);

            builder.Property(pi => pi.RequiresWaterChange)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pi => pi.FishDiedCount);

            builder.Property(pi => pi.CorrectiveActions)
                .HasMaxLength(2000);

            builder.Property(pi => pi.Notes)
                .HasMaxLength(2000);

            builder.HasOne(pi => pi.Incident)
                .WithMany(i => i.PondIncidents)
                .HasForeignKey(pi => pi.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Pond)
                .WithMany(p => p.PondIncidents)
                .HasForeignKey(pi => pi.PondId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pi => pi.IncidentId);
            builder.HasIndex(pi => pi.PondId);
        }
    }
}
