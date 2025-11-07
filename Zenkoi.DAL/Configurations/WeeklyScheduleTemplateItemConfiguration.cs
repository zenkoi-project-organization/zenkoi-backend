using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class WeeklyScheduleTemplateItemConfiguration : IEntityTypeConfiguration<WeeklyScheduleTemplateItem>
{
    public void Configure(EntityTypeBuilder<WeeklyScheduleTemplateItem> builder)
    {
        builder.ToTable("WeeklyScheduleTemplateItems");
        builder.HasKey(wsti => wsti.Id);
        builder.Property(wsti => wsti.Id).UseIdentityColumn();

        builder.Property(wsti => wsti.WeeklyScheduleTemplateId)
            .IsRequired();

        builder.Property(wsti => wsti.TaskTemplateId)
            .IsRequired();

        builder.Property(wsti => wsti.DayOfWeek)
            .IsRequired();

        builder.Property(wsti => wsti.StartTime)
            .IsRequired();

        builder.Property(wsti => wsti.EndTime)
            .IsRequired();

        builder.HasOne(wsti => wsti.WeeklyScheduleTemplate)
            .WithMany(wst => wst.TemplateItems)
            .HasForeignKey(wsti => wsti.WeeklyScheduleTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wsti => wsti.TaskTemplate)
            .WithMany()
            .HasForeignKey(wsti => wsti.TaskTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
