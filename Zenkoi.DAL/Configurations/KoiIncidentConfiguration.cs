using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class KoiIncidentConfiguration : IEntityTypeConfiguration<KoiIncident>
    {
        public void Configure(EntityTypeBuilder<KoiIncident> builder)
        {
            builder.ToTable("KoiIncidents");
            builder.HasKey(ki => ki.Id);
            builder.Property(ki => ki.Id).UseIdentityColumn();

            builder.Property(ki => ki.IncidentId)
                .IsRequired();

            builder.Property(ki => ki.KoiFishId)
                .IsRequired();

            builder.Property(ki => ki.AffectedStatus)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ki => ki.Severity)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ki => ki.SpecificSymptoms)
                .HasMaxLength(1000);

            builder.Property(ki => ki.RequiresTreatment)
                .IsRequired();

            builder.Property(ki => ki.IsIsolated)
                .IsRequired();

            builder.Property(ki => ki.AffectedFrom)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ki => ki.RecoveredAt);

            builder.Property(ki => ki.TreatmentNotes)
                .HasMaxLength(2000);

            builder.HasOne(ki => ki.Incident)
                .WithMany(i => i.KoiIncidents)
                .HasForeignKey(ki => ki.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ki => ki.KoiFish)
                .WithMany(k => k.KoiIncidents)
                .HasForeignKey(ki => ki.KoiFishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ki => ki.IncidentId);
            builder.HasIndex(ki => ki.KoiFishId);
        }
    }
}
