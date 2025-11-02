using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class StaffAssignmentConfiguration : IEntityTypeConfiguration<StaffAssignment>
{
    public void Configure(EntityTypeBuilder<StaffAssignment> builder)
    {
        builder.ToTable("StaffAssignments");

        builder.HasKey(sa => new { sa.WorkScheduleId, sa.StaffId });

        builder.Property(sa => sa.WorkScheduleId)
            .IsRequired();

        builder.Property(sa => sa.StaffId)
            .IsRequired();

        builder.Property(sa => sa.CompletionNotes)
            .HasMaxLength(1000);

        builder.Property(sa => sa.CompletedAt);

        builder.HasOne(sa => sa.WorkSchedule)
            .WithMany(ws => ws.StaffAssignments)
            .HasForeignKey(sa => sa.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sa => sa.Staff)
            .WithMany()
            .HasForeignKey(sa => sa.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sa => sa.StaffId);
        builder.HasIndex(sa => sa.WorkScheduleId);
    }
}