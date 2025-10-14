using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.ToTable("Incidents");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).UseIdentityColumn();

            builder.Property(i => i.IncidentTypeId)
                .IsRequired();

            builder.Property(i => i.IncidentTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(i => i.Severity)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.OccurredAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.ResolvedAt);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.ReportedByUserId)
                .IsRequired();

            builder.Property(i => i.ResolvedByUserId);

            builder.Property(i => i.ResolutionNotes)
                .HasMaxLength(2000);

            builder.HasOne(i => i.IncidentType)
                .WithMany(it => it.Incidents)
                .HasForeignKey(i => i.IncidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.ReportedBy)
                .WithMany()
                .HasForeignKey(i => i.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.ResolvedBy)
                .WithMany()
                .HasForeignKey(i => i.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.KoiIncidents)
                .WithOne(ki => ki.Incident)
                .HasForeignKey(ki => ki.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.PondIncidents)
                .WithOne(pi => pi.Incident)
                .HasForeignKey(pi => pi.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.IncidentTypeId);
            builder.HasIndex(i => i.ReportedByUserId);
            builder.HasIndex(i => i.OccurredAt);
        }
    }
}
