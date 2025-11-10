using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class IncidentTypeConfiguration : IEntityTypeConfiguration<IncidentType>
    {
        public void Configure(EntityTypeBuilder<IncidentType> builder)
        {
            builder.ToTable("IncidentTypes");
            builder.HasKey(it => it.Id);
            builder.Property(it => it.Id).UseIdentityColumn();

            builder.Property(it => it.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(it => it.Description)
                .HasMaxLength(500);

            builder.Property(it => it.DefaultSeverity)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(it => it.RequiresQuarantine)
                .HasDefaultValue(false);

            builder.Property(it => it.AffectsBreeding)
                .HasDefaultValue(false);

            builder.HasMany(it => it.Incidents)
                .WithOne(i => i.IncidentType)
                .HasForeignKey(i => i.IncidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(it => it.Name).IsUnique();
        }
    }
}
