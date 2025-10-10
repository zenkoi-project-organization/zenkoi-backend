using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zenkoi.DAL.Configurations;
using Zenkoi.DAL.Entities;

namespace Zenkoi.DAL.EF
{
	public partial class ZenKoiContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
	{

		public ZenKoiContext(DbContextOptions<ZenKoiContext> options)
		: base(options)
		{

		}

		#region DbSet
		public DbSet<UserDetail> UserDetail { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("dbo");
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentTransactionConfiguration());
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
			{
				entity.ToTable("UserLogin");
				entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
			});

			modelBuilder.Entity<IdentityUserToken<int>>(entity =>
			{
				entity.ToTable("UserToken");
				entity.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
			});

			modelBuilder.Entity<IdentityRole<int>>(entity =>
			{
				entity.ToTable("Roles");
			});

			modelBuilder.Entity<IdentityUserRole<int>>(entity =>
			{
				entity.ToTable("UserRoles");
				entity.HasKey(ur => new { ur.UserId, ur.RoleId });
			});

			modelBuilder.Entity<RefreshToken>(entity =>
			{
				entity.ToTable("RefreshToken");
				entity.HasKey(r => r.Id);
				entity.Property(r => r.JwtId).IsRequired();
				entity.Property(r => r.Token).IsRequired();
				entity.Property(r => r.IssuedAt).IsRequired();
				entity.Property(r => r.ExpiredAt).IsRequired();
				entity.HasOne(r => r.ApplicationUser)
					.WithMany()
					.HasForeignKey(r => r.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			
		}
	}

}
