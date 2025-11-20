using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
    public class KoiFavoriteConfiguration : IEntityTypeConfiguration<KoiFavorite>
    {
        public void Configure(EntityTypeBuilder<KoiFavorite> builder)
        {
            builder.ToTable("KoiFavorites", "dbo");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseIdentityColumn();

            builder.Property(k => k.CreatedAt)
                .IsRequired();

            builder.HasOne(k => k.User)
                .WithMany()
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(k => k.KoiFish)
                .WithMany()
                .HasForeignKey(k => k.KoiFishId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(k => new { k.UserId, k.KoiFishId })
                .IsUnique();
        }
    }
}
