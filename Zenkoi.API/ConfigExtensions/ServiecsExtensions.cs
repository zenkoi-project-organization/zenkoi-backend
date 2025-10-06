using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Zenkoi.BLL.Helpers.Mapper;
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
				await context.Database.EnsureDeletedAsync();
				await context.Database.EnsureCreatedAsync();
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
			#region Seeding UserDetail
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
            #endregion

            if (!context.UserRoles.Any(x => x.RoleId == 1))
			{
				await context.UserRoles.AddAsync(
					// Manager
					new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
				);
				await context.SaveChangesAsync();
			}
	
			
			}
		}		
	
}
