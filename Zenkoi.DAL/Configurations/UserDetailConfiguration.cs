using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
	public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
	{
		public void Configure(EntityTypeBuilder<UserDetail> builder)
		{
			builder.ToTable("UserDetails");
			builder.HasKey(ud => ud.Id);
			builder.Property(ud => ud.Id).UseIdentityColumn();

			builder.Property(ud => ud.DateOfBirth)
				   .IsRequired(false);
			builder.Property(ud => ud.Gender)
				   .IsRequired();
			builder.Property(ud => ud.AvatarURL)
				   .HasMaxLength(500)
				   .IsRequired(false);
			builder.Property(ud => ud.Address)
				   .HasMaxLength(500)
				   .IsRequired(false);

			builder.HasOne(ud => ud.User)
				   .WithOne(u => u.UserDetail)
				   .HasForeignKey<UserDetail>(ud => ud.ApplicationUserId)
				   .IsRequired();
		}
	}
}
