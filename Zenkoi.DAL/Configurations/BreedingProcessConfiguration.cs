using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class BreedingProcessConfiguration : IEntityTypeConfiguration<BreedingProcess>
    {
        public void Configure(EntityTypeBuilder<BreedingProcess> builder)
        {
            builder.ToTable("BreedingProcesses");
            builder.HasKey(bp => bp.Id);
            builder.Property(bp => bp.Id).UseIdentityColumn();

            builder.Property(bp => bp.MaleKoiId)
                .IsRequired();

            builder.Property(bp => bp.FemaleKoiId)
                .IsRequired();

            builder.Property(bp => bp.PondId)
                .IsRequired();

            builder.Property(bp => bp.StartDate)
                .IsRequired();

            builder.Property(bp => bp.EndDate);

            builder.Property(bp => bp.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(bp => bp.Note)
                .HasMaxLength(1000);

            builder.Property(bp => bp.Result)
                .HasMaxLength(1000);

            builder.Property(bp => bp.TotalFishQualified);

            builder.Property(bp => bp.TotalPackage);

            builder.HasOne(bp => bp.MaleKoi)
                .WithMany()
                .HasForeignKey(bp => bp.MaleKoiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.FemaleKoi)
                .WithMany()
                .HasForeignKey(bp => bp.FemaleKoiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.Pond)
                .WithMany()
                .HasForeignKey(bp => bp.PondId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bp => bp.KoiFishes)
                .WithOne(k => k.BreedingProcess)
                .HasForeignKey(k => k.BreedingProcessId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
