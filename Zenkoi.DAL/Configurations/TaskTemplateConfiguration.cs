using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class TaskTemplateConfiguration : IEntityTypeConfiguration<TaskTemplate>
{
    public void Configure(EntityTypeBuilder<TaskTemplate> builder)
    {
        builder.ToTable("TaskTemplates");
        builder.HasKey(tt => tt.Id);
        builder.Property(tt => tt.Id).UseIdentityColumn();

        builder.Property(tt => tt.TaskName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tt => tt.Description)
            .HasMaxLength(1000);

        builder.Property(tt => tt.DefaultDuration)
            .IsRequired();

        builder.Property(tt => tt.IsRecurring)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tt => tt.RecurrenceRule)
            .HasMaxLength(500);

        builder.Property(tt => tt.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tt => tt.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(tt => tt.UpdatedAt);

        builder.HasMany(tt => tt.WorkSchedules)
            .WithOne(ws => ws.TaskTemplate)
            .HasForeignKey(ws => ws.TaskTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tt => tt.TaskName);
        builder.HasIndex(tt => tt.IsDeleted);
        builder.HasIndex(tt => tt.IsRecurring);
    }
}
