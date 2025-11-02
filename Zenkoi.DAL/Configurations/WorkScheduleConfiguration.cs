using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules");
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id).UseIdentityColumn();

        builder.Property(ws => ws.TaskTemplateId)
            .IsRequired();

        builder.Property(ws => ws.ScheduledDate)
            .IsRequired();

        builder.Property(ws => ws.StartTime)
            .IsRequired();

        builder.Property(ws => ws.EndTime)
            .IsRequired();

        builder.Property(ws => ws.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Enums.WorkTaskStatus.Pending);

        builder.Property(ws => ws.Notes)
            .HasMaxLength(1000);

        builder.Property(ws => ws.CreatedBy)
            .IsRequired();

        builder.Property(ws => ws.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ws => ws.UpdatedAt);

        // Relationships
        builder.HasOne(ws => ws.TaskTemplate)
            .WithMany(t => t.WorkSchedules)
            .HasForeignKey(ws => ws.TaskTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ws => ws.Creator)
            .WithMany()
            .HasForeignKey(ws => ws.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ws => ws.StaffAssignments)
            .WithOne(sa => sa.WorkSchedule)
            .HasForeignKey(sa => sa.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ws => ws.PondAssignments)
            .WithOne(pa => pa.WorkSchedule)
            .HasForeignKey(pa => pa.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ws => ws.TaskTemplateId);
        builder.HasIndex(ws => ws.ScheduledDate);
        builder.HasIndex(ws => ws.Status);
        builder.HasIndex(ws => ws.CreatedBy);
        builder.HasIndex(ws => new { ws.ScheduledDate, ws.Status });
    }
}
