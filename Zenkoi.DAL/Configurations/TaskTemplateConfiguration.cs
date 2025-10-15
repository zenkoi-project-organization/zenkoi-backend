using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class TaskTemplateConfiguration : IEntityTypeConfiguration<TaskTemplate>
    {
        public void Configure(EntityTypeBuilder<TaskTemplate> builder)
        {
            builder.ToTable("TaskTemplates");
            builder.HasKey(tt => tt.Id);
            builder.Property(tt => tt.Id).UseIdentityColumn();

            builder.Property(tt => tt.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(tt => tt.Description)
                .HasMaxLength(1000);

            builder.Property(tt => tt.PondId);

            builder.Property(tt => tt.AssignedToUserId)
                .IsRequired();

            builder.Property(tt => tt.ScheduledAt)
                .IsRequired();

            builder.Property(tt => tt.IsRecurring)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(tt => tt.RecurrenceRule)
                .HasMaxLength(200);

            builder.HasOne(tt => tt.Pond)
                .WithMany(p => p.Tasks)
                .HasForeignKey(tt => tt.PondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tt => tt.AssignedTo)
                .WithMany()
                .HasForeignKey(tt => tt.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(tt => tt.WorkSchedules)
                .WithOne(ws => ws.TaskTemplate)
                .HasForeignKey(ws => ws.TaskTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(tt => tt.AssignedToUserId);
            builder.HasIndex(tt => tt.ScheduledAt);
        }
    }
}
