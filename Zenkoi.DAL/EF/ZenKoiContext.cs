using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
        public DbSet<KoiFish> KoiFishes { get; set; }
        public DbSet<Variety> Varieties { get; set; }
        public DbSet<PondType> PondTypes {  get; set; }
        public DbSet<Pond> Ponds { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<BreedingProcess> BreedingProcesses {  get; set; }
        public DbSet<EggBatch> EggBatches { get; set; }
        public DbSet<IncubationDailyRecord> IncubationDailyRecords { get; set; }
        public DbSet<FryFish> FryFishes { get; set; }
        public DbSet<FrySurvivalRecord> FrySurvivalRecords { get; set; }
        public DbSet<ClassificationStage> ClassificationStages { get; set; }
        public DbSet<ClassificationRecord> ClassificationRecords { get; set; }
        
        // New entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }
        public DbSet<KoiIncident> KoiIncidents { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PacketFish> PacketFishes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PondIncident> PondIncidents { get; set; }
        public DbSet<PondPacketFish> PondPacketFishes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<TaskTemplate> TaskTemplates { get; set; }
        public DbSet<VarietyPacketFish> VarietyPacketFishes { get; set; }
        public DbSet<WaterAlert> WaterAlerts { get; set; }
        public DbSet<WaterParameterRecord> WaterParameterRecords { get; set; }
        public DbSet<WaterParameterThreshold> WaterParameterThresholds { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
 
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
            
            // Apply all new configurations
            modelBuilder.ApplyConfiguration(new AreaConfiguration());
            modelBuilder.ApplyConfiguration(new BreedingProcessConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new IncidentConfiguration());
            modelBuilder.ApplyConfiguration(new IncidentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new KoiFishConfiguration());
            modelBuilder.ApplyConfiguration(new KoiIncidentConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PacketFishConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new PondConfiguration());
            modelBuilder.ApplyConfiguration(new PondIncidentConfiguration());
            modelBuilder.ApplyConfiguration(new PondPacketFishConfiguration());
            modelBuilder.ApplyConfiguration(new PondTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionConfiguration());
            modelBuilder.ApplyConfiguration(new TaskTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new VarietyConfiguration());
            modelBuilder.ApplyConfiguration(new VarietyPacketFishConfiguration());
            modelBuilder.ApplyConfiguration(new WaterAlertConfiguration());
            modelBuilder.ApplyConfiguration(new WaterParameterRecordConfiguration());
            modelBuilder.ApplyConfiguration(new WaterParameterThresholdConfiguration());
            modelBuilder.ApplyConfiguration(new WorkScheduleConfiguration());
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
            modelBuilder.Entity<BreedingProcess>()
			  .HasOne(bp => bp.MaleKoi)
			  .WithMany()
			  .HasForeignKey(bp => bp.MaleKoiId)
			  .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BreedingProcess>()
                .HasOne(bp => bp.FemaleKoi)
                .WithMany()
                .HasForeignKey(bp => bp.FemaleKoiId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BreedingProcess>()
            .HasOne(bp => bp.Pond)
            .WithMany()
            .HasForeignKey(bp => bp.PondId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BreedingProcess>()
                .HasMany(bp => bp.KoiFishes)
                .WithOne(k => k.BreedingProcess)
                .HasForeignKey(k => k.BreedingProcessId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<KoiFish>()
              .Property(k => k.Images)
              .HasConversion(
                  v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                  v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
              );

            modelBuilder.Entity<KoiFish>()
                .Property(k => k.Videos)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );

        }
	}

}
