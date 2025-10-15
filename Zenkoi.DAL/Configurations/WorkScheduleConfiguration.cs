using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
    {
        public void Configure(EntityTypeBuilder<WorkSchedule> builder)
        {
            builder.ToTable("WorkSchedules");
            builder.HasKey(ws => ws.Id);
            builder.Property(ws => ws.Id).UseIdentityColumn();

            builder.Property(ws => ws.StaffId)
                .IsRequired();

            builder.Property(ws => ws.TaskTemplateId)
                .IsRequired();

            builder.Property(ws => ws.WorkDate)
                .IsRequired();

            builder.Property(ws => ws.StartTime)
                .HasDefaultValue(TimeSpan.Zero);

            builder.Property(ws => ws.EndTime);

            builder.Property(ws => ws.CheckedInAt);

            builder.Property(ws => ws.CheckedOutAt);

            builder.Property(ws => ws.Notes)
                .HasMaxLength(1000);

            builder.Property(ws => ws.ManagerNotes)
                .HasMaxLength(1000);

            builder.Property(ws => ws.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ws => ws.CreatedByUserId)
                .IsRequired();

            builder.HasOne(ws => ws.Staff)
                .WithMany()
                .HasForeignKey(ws => ws.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ws => ws.TaskTemplate)
                .WithMany(tt => tt.WorkSchedules)
                .HasForeignKey(ws => ws.TaskTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ws => ws.CreatedBy)
                .WithMany()
                .HasForeignKey(ws => ws.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ws => ws.StaffId);
            builder.HasIndex(ws => ws.TaskTemplateId);
            builder.HasIndex(ws => ws.WorkDate);
            builder.HasIndex(ws => ws.CreatedByUserId);
        }
    }
}
