using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations;

public class WeeklyScheduleTemplateConfiguration : IEntityTypeConfiguration<WeeklyScheduleTemplate>
{
    public void Configure(EntityTypeBuilder<WeeklyScheduleTemplate> builder)
    {
        builder.ToTable("WeeklyScheduleTemplates");
        builder.HasKey(wst => wst.Id);
        builder.Property(wst => wst.Id).UseIdentityColumn();

        builder.Property(wst => wst.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wst => wst.Description)
            .HasMaxLength(500);

        builder.Property(wst => wst.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(wst => wst.CreatedAt)
            .IsRequired();

        builder.Property(wst => wst.UpdatedAt);

        builder.HasMany(wst => wst.TemplateItems)
            .WithOne(wsti => wsti.WeeklyScheduleTemplate)
            .HasForeignKey(wsti => wsti.WeeklyScheduleTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
