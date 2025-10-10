using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.Configurations
{
	public class PaymentMethodConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.ToTable("PaymentMethods");
			builder.HasKey(pm => pm.Id);
			builder.Property(pm => pm.Id).UseIdentityColumn();

			builder.Property(pm => pm.PaymentInfo)
				   .IsRequired()
				   .HasMaxLength(500);
			builder.Property(pm => pm.BankName)
				   .HasMaxLength(200);
			builder.Property(pm => pm.IsDefault)
				   .IsRequired();

			builder.HasOne(pm => pm.User)
				   .WithMany()
				   .HasForeignKey(pm => pm.UserId)
				   .IsRequired();
		}
	}
}
