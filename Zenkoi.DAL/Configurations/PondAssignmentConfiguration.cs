using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class PondAssignmentConfiguration : IEntityTypeConfiguration<PondAssignment>
{
    public void Configure(EntityTypeBuilder<PondAssignment> builder)
    {
        builder.ToTable("PondAssignments");

        builder.HasKey(pa => new { pa.WorkScheduleId, pa.PondId });

        builder.Property(pa => pa.WorkScheduleId)
            .IsRequired();

        builder.Property(pa => pa.PondId)
            .IsRequired();

        builder.HasOne(pa => pa.WorkSchedule)
            .WithMany(ws => ws.PondAssignments)
            .HasForeignKey(pa => pa.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.Pond)
            .WithMany()
            .HasForeignKey(pa => pa.PondId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pa => pa.PondId);
        builder.HasIndex(pa => pa.WorkScheduleId);
    }
}