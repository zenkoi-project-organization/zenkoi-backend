using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Zenkoi.BLL.Helpers.Mapper;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.EF;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.API.ConfigExtensions
{
	public static class ServiecsExtensions
	{
		//Unit Of Work
		public static void AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		//RepoBase
		public static void AddRepoBase(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));
		}

		//BLL Services
		public static void AddBLLServices(this IServiceCollection services)
		{
			services.Scan(scan => scan
					.FromAssemblies(Assembly.Load("Zenkoi.BLL"))
					.AddClasses(classes => classes.Where(type => type.Namespace == $"Zenkoi.BLL.Services.Implements" && type.Name.EndsWith("Service")))
					.AsImplementedInterfaces()
					.WithScopedLifetime());

        }

		// Auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}

		// Swagger
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "ZenKoi API", Version = "v1" });
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							}
						},
						new string[]{}
					}
				});
			});
		}

		//Seed Data
		public static async Task SeedData(this IServiceProvider serviceProvider)
		{
			using var context = new ZenKoiContext(serviceProvider.GetRequiredService<DbContextOptions<ZenKoiContext>>());

			// Lấy môi trường hiện tại (Development, Production)
			var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

			if (env.IsDevelopment())
			{
			//	await TruncateAllTablesExceptMigrationHistory(context);
			}

			#region Seeding Roles
			if (!context.Roles.Any())
			{
				await context.Roles.AddRangeAsync(
					new IdentityRole<int> { Name = "Manager", NormalizedName = "Manager" },
					new IdentityRole<int> { Name = "FarmStaff", NormalizedName = "FARMSTAFF" },
					new IdentityRole<int> { Name = "SaleStaff", NormalizedName = "SALESTAFF" },
					new IdentityRole<int> { Name = "Customer", NormalizedName = "CUSTOMER" }
				);
				await context.SaveChangesAsync();
			}
			#endregion

			#region Seeding Manager
			if (!context.Users.Any(x => x.Role == Role.Manager))
			{
				// Pass: Admin@123
				var manager = new ApplicationUser { FullName = "manager", Role = Role.Manager, UserName = "manager", NormalizedUserName = "MANAGER", Email = "manager@email.com", NormalizedEmail = "MANAGER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP", ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a", PhoneNumber = "0999999999", LockoutEnabled = true };
				await context.Users.AddAsync(manager);
				await context.SaveChangesAsync();
				await context.SaveChangesAsync();
			}
			#endregion
			#region Seeding Data
			if (!context.UserDetail.Any())
			{
				await context.UserDetail.AddRangeAsync(
					// Admin
					new UserDetail
					{
						ApplicationUserId = 1,
						DateOfBirth = new DateTime(1985, 3, 15),
						Gender = Gender.Male,
						AvatarURL = "http://res.cloudinary.com/detykxgzs/image/upload/v1759744354/h5mkb2lj97m9w2q4rjnp.png",
						Address = "123 Nguyễn Văn Cừ, Quận 5, TP.HCM"
					}
					);
			}


			if (!context.UserRoles.Any(x => x.RoleId == 1))
			{
				await context.UserRoles.AddAsync(
					// Manager
					new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
				);
				await context.SaveChangesAsync();
			}
			if (!context.Areas.Any())
			{
				await context.Areas.AddRangeAsync(
					new Area
					{
						AreaName = "Khu A",
						TotalAreaSQM = 500.5,
						Description = "Khu nuôi cá koi cao cấp"
					}
				);
				await context.SaveChangesAsync();
			}
			if (!context.Varieties.Any())
			{
				await context.Varieties.AddRangeAsync(
				   new Variety
				   {
					   VarietyName = "Kohaku",
					   Characteristic = "Thân trắng với các mảng đỏ",
					   OriginCountry = "Nhật Bản"
				   },
				   new Variety
				   {
					   VarietyName = "Sanke",
					   Characteristic = "Thân trắng, đốm đỏ và đen",
					   OriginCountry = "Nhật Bản"
				   }
			   );
			}
			if (!context.Ponds.Any())
			{
				await context.Ponds.AddRangeAsync(
					 new Pond
					 {
						 PondName = "Pond A1",
						 PondTypeId = 1,
						 AreaId = 1,
						 Location = "Khu A - Góc Đông",
						 PondStatus = PondStatus.Empty,
						 CapacityLiters = 10000,
						 DepthMeters = 1.5,
						 CreatedAt = DateTime.Now
					 }
				);
			}
			if (!context.PondTypes.Any())
			{
				await context.PondTypes.AddRangeAsync(
					 new PondType
					 {
						 TypeName = "Ao sinh sản",
						 Description = "Ao dành cho cá bố mẹ sinh sản",
						 RecommendedCapacity = 8000
					 },
						new PondType
						{
							TypeName = "Ao ương cá bột",
							Description = "Ao ương cá con sau khi nở",
							RecommendedCapacity = 5000
						}
					);
			}
			if (context.KoiFishes.Any())
			{
				await context.KoiFishes.AddRangeAsync(
					new KoiFish
					{
                        PondId = 1,
                        VarietyId = 1,
                        RFID = "RFID-001",
                        Size = 25.5,
                        BirthDate = new DateTime(2023, 3, 15),
                        Gender = Gender.Male,
                        HealthStatus = HealthStatus.Healthy,
                        Images = new List<string>
    {
        "https://res.cloudinary.com/demo/image/upload/sample.jpg",
        "https://res.cloudinary.com/demo/image/upload/sample2.jpg"
    },
                        Videos = new List<string>
    {
        "https://res.cloudinary.com/demo/video/upload/koi_video.mp4"
    },
                        SellingPrice = 3500000,
                        BodyShape = "Slim and symmetrical",
                        Description = "Kohaku koi with vibrant red markings",
                        CreatedAt = DateTime.Now
                    }
					);
			}
			if (!context.BreedingProcesses.Any())
			{
				await context.BreedingProcesses.AddRangeAsync(
				new BreedingProcess
				{
					MaleKoiId = 1,
					FemaleKoiId = 2,
					PondId = 1,
					StartDate = new DateTime(2025, 1, 10),
					EndDate = new DateTime(2025, 2, 15),
					Status = BreedingStatus.Complete,
					Note = "Quá trình sinh sản thành công, nhiều trứng nở khỏe mạnh.",
					Result = BreedingResult.Success,
					TotalFishQualified = 120,
					TotalPackage = 3
				}
				);
			}
			if (!context.EggBatches.Any())
			{
				await context.EggBatches.AddRangeAsync(
					new EggBatch
					{
						BreedingProcessId = 1,
						PondId = 1,
						Quantity = 5000,
						FertilizationRate = 0.85,
						Status = EggBatchStatus.Success,
						SpawnDate = new DateTime(2025, 2, 16),
						HatchingTime = new DateTime(2025, 2, 22)
					}
				);
			}
			if (!context.IncubationDailyRecords.Any())
			{
				await context.IncubationDailyRecords.AddRangeAsync(
					new IncubationDailyRecord
					{
						EggBatchId = 1,
						DayNumber = 1,
						HealthyEggs = 4800,
						RottenEggs = 200,
						HatchedEggs = 0
					},
					new IncubationDailyRecord
					{
						EggBatchId = 1,
						DayNumber = 3,
						HealthyEggs = 4700,
						RottenEggs = 300,
						HatchedEggs = 0
					}
					);
			}
			if (!context.FryFishes.Any())
			{
				await context.FryFishes.AddRangeAsync(
					new FryFish
					{
						BreedingProcessId = 1,
						PondId = 1,
						InitialCount = 4500,
						Status = FryFishStatus.Growing,
						CurrentSurvivalRate = 0.93
					},
					new FryFish
					{
						BreedingProcessId = 2,
						PondId = 2,
						InitialCount = 6200,
						Status = FryFishStatus.Completed,
						CurrentSurvivalRate = 0.89
					}
					);
			}
			if (!context.FrySurvivalRecords.Any())
			{
				await context.FrySurvivalRecords.AddRangeAsync(
					new FrySurvivalRecord
					{
						FryFishId = 1,
						DayNumber = 1,
						SurvivalRate = 0.95,
						CountAlive = 4275
					},
					new FrySurvivalRecord
					{
						FryFishId = 1,
						DayNumber = 3,
						SurvivalRate = 0.93,
						CountAlive = 4185
					}
					);
			}
			if (!context.ClassificationStages.Any())
			{
				await context.ClassificationStages.AddRangeAsync(
					new ClassificationStage
					{
						BreedingProcessId = 1,
						TotalCount = 4200,
						HighQualifiedCount = 800,
						QualifiedCount = 2500,
						UnqualifiedCount = 900,
						Notes = "Phân loại lần đầu — nhóm cá khoẻ mạnh, màu sắc rõ nét chiếm khoảng 20%."
					}
					);
			}
			if (!context.ClassificationRecords.Any())
			{
				await context.ClassificationRecords.AddRangeAsync(
					new ClassificationRecord
					{
						ClassificationStageId = 1,
						StageNumber = 1,
						CreateAt = DateTime.Now,
						HighQualifiedCount = 300,
						QualifiedCount = 500,
						UnqualifiedCount = 200,
						Notes = "Đánh giá ban đầu, cá khỏe mạnh đạt 30%."
					}
					);
			}
		}
		#endregion
	
        private static async Task TruncateAllTablesExceptMigrationHistory(ZenKoiContext context)
        {
            await context.Database.ExecuteSqlRawAsync(@"
            -- Set QUOTED_IDENTIFIER ON cho toàn bộ batch
            SET QUOTED_IDENTIFIER ON;

            -- Disable tất cả constraints
            DECLARE @sql NVARCHAR(MAX) = N'';
            
            SELECT @sql += 'ALTER TABLE ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ' NOCHECK CONSTRAINT ALL;'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE';
            
            EXEC sp_executesql @sql;

            -- Xóa dữ liệu tất cả bảng NGOẠI TRỪ __EFMigrationsHistory
            SET @sql = N'';
            
            SELECT @sql += 'DELETE FROM ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ';'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE' 
            AND TABLE_NAME != '__EFMigrationsHistory';
            
            EXEC sp_executesql @sql;

            -- Re-enable constraints
            SET @sql = N'';
            
            SELECT @sql += 'ALTER TABLE ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ' WITH CHECK CHECK CONSTRAINT ALL;'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE';
            
            EXEC sp_executesql @sql;

            -- Reset Identity columns
            SET @sql = N'';
            
            SELECT @sql += 
                'IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(''' 
                + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ''')) ' +
                'DBCC CHECKIDENT (''' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ''', RESEED, 0);'
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE' 
            AND TABLE_NAME != '__EFMigrationsHistory';
            
            EXEC sp_executesql @sql;
        ");
		}
	}
}