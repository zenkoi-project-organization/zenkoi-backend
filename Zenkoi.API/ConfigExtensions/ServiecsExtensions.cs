using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Zenkoi.BLL.Helpers.Mapper;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.BLL.WebSockets;
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
            services.AddScoped<ExpoPushNotificationService>();
            services.AddSingleton<AlertWebSocketEndpoint>();
            services.AddSingleton<WebSocketConnectionManager>();

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
    //          await TruncateAllTablesExceptMigrationHistory(context);
            }

            #region Seeding Roles
            if (!context.Roles.Any())
            {
                await context.Roles.AddRangeAsync(
                    new IdentityRole<int> { Name = "Manager", NormalizedName = "MANAGER" },
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
                var manager = new ApplicationUser
                {
                    FullName = "manager",
                    Role = Role.Manager,
                    UserName = "manager",
                    NormalizedUserName = "MANAGER",
                    Email = "manager@email.com",
                    NormalizedEmail = "MANAGER@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a",
                    PhoneNumber = "0999999999",
                    LockoutEnabled = true
                };

                // Thêm tài khoản Manager mới từ JSON
                var manager2 = new ApplicationUser
                {
                    FullName = "Huy",
                    Role = Role.Manager,
                    UserName = "dinhhoa",
                    NormalizedUserName = "DINHHOA",
                    Email = "885relative@powerscrews.com",
                    NormalizedEmail = "885RELATIVE@POWERSCREWS.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", // bạn có thể thay hash thật nếu muốn
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "09463575689",
                    LockoutEnabled = true
                };

                await context.Users.AddRangeAsync(manager, manager2);
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Customer Users            
            ApplicationUser customer1 = null;
            ApplicationUser customer2 = null;
            ApplicationUser customer3 = null;

            if (!context.Users.Any(x => x.Role == Role.Customer))
            {
                customer1 = new ApplicationUser
                {
                    FullName = "Nguyễn Văn An",
                    Role = Role.Customer,
                    UserName = "customer1",
                    NormalizedUserName = "CUSTOMER1",
                    Email = "customer1@email.com",
                    NormalizedEmail = "CUSTOMER1@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0987654321",
                    LockoutEnabled = true
                };

                customer2 = new ApplicationUser
                {
                    FullName = "Trần Thị Bình",
                    Role = Role.Customer,
                    UserName = "customer2",
                    NormalizedUserName = "CUSTOMER2",
                    Email = "customer2@email.com",
                    NormalizedEmail = "CUSTOMER2@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0912345678",
                    LockoutEnabled = true
                };

                customer3 = new ApplicationUser
                {
                    FullName = "Lê Văn Cường",
                    Role = Role.Customer,
                    UserName = "customer3",
                    NormalizedUserName = "CUSTOMER3",
                    Email = "customer3@email.com",
                    NormalizedEmail = "CUSTOMER3@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0901234567",
                    LockoutEnabled = true
                };

                await context.Users.AddRangeAsync(customer1, customer2, customer3);
                await context.SaveChangesAsync();

            }
            else
            {
                var customers = await context.Users.Where(x => x.Role == Role.Customer).OrderBy(x => x.Id).Take(3).ToListAsync();
                if (customers.Count >= 3)
                {
                    customer1 = customers[0];
                    customer2 = customers[1];
                    customer3 = customers[2];
                }
            }
            #endregion

            #region Seeding FarmStaff Users
            if (!context.Users.Any(x => x.Role == Role.FarmStaff))
            {
                var staff1 = new ApplicationUser
                {
                    FullName = "Nguyễn Văn Bình",
                    Role = Role.FarmStaff,
                    UserName = "staff1",
                    NormalizedUserName = "STAFF1",
                    Email = "staff1@email.com",
                    NormalizedEmail = "STAFF1@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0988888881",
                    LockoutEnabled = true
                };

                var staff2 = new ApplicationUser
                {
                    FullName = "Trần Thị Cẩm",
                    Role = Role.FarmStaff,
                    UserName = "staff2",
                    NormalizedUserName = "STAFF2",
                    Email = "staff2@email.com",
                    NormalizedEmail = "STAFF2@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0988888882",
                    LockoutEnabled = true
                };

                var staff3 = new ApplicationUser
                {
                    FullName = "Lê Văn Dũng",
                    Role = Role.FarmStaff,
                    UserName = "staff3",
                    NormalizedUserName = "STAFF3",
                    Email = "staff3@email.com",
                    NormalizedEmail = "STAFF3@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "0988888883",
                    LockoutEnabled = true
                };

                // 👇 Thêm tài khoản FarmStaff mới từ JSON
                var staff4 = new ApplicationUser
                {
                    FullName = "Huy",
                    Role = Role.FarmStaff,
                    UserName = "Huy",
                    NormalizedUserName = "HUY",
                    Email = "huy@gmailcom",
                    NormalizedEmail = "HUY@GMAILCOM",
                    PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "09463575689",
                    LockoutEnabled = true
                };

                await context.Users.AddRangeAsync(staff1, staff2, staff3, staff4);
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
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding UserRoles
            if (!context.UserRoles.Any(x => x.RoleId == 1))
            {
                await context.UserRoles.AddAsync(
                    // Manager
                    new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
                );
                await context.SaveChangesAsync();
            }

            if (customer1 != null && customer2 != null && customer3 != null)
            {
                if (!context.UserRoles.Any(x => x.UserId == customer1.Id || x.UserId == customer2.Id || x.UserId == customer3.Id))
                {
                    await context.UserRoles.AddRangeAsync(
                        new IdentityUserRole<int> { UserId = customer1.Id, RoleId = 4 },
                        new IdentityUserRole<int> { UserId = customer2.Id, RoleId = 4 },
                        new IdentityUserRole<int> { UserId = customer3.Id, RoleId = 4 }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding Customers

            if (customer1 != null && customer2 != null && customer3 != null)
            {
                if (!context.Customers.Any())
                {
                    await context.Customers.AddRangeAsync(
                        new Customer
                        {
                            Id = customer1.Id,
                            ContactNumber = "0987654321",
                            TotalOrders = 2,
                            TotalSpent = 15000000,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Customer
                        {
                            Id = customer2.Id,
                            ContactNumber = "0912345678",
                            TotalOrders = 1,
                            TotalSpent = 12000000,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Customer
                        {
                            Id = customer3.Id,
                            ContactNumber = "0901234567",
                            TotalOrders = 0,
                            TotalSpent = 0,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding CustomerAddresses
            if (customer1 != null && customer2 != null && customer3 != null)
            {
                if (!context.CustomerAddresses.Any())
                {
                    await context.CustomerAddresses.AddRangeAsync(
                        new CustomerAddress
                        {
                            CustomerId = customer1.Id,
                            FullAddress = "123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận 1",
                            Ward = "Phường Bến Nghé",
                            StreetAddress = "123 Đường Lê Lợi",
                            Latitude = 10.7769m,
                            Longitude = 106.7009m,
                            RecipientPhone = "0987654321",
                            IsDefault = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new CustomerAddress
                        {
                            CustomerId = customer1.Id,
                            FullAddress = "456 Đường Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận 1",
                            Ward = "Phường Bến Nghé",
                            StreetAddress = "456 Đường Nguyễn Huệ",
                            Latitude = 10.7743m,
                            Longitude = 106.7010m,
                            RecipientPhone = "0987654321",
                            IsDefault = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new CustomerAddress
                        {
                            CustomerId = customer2.Id,
                            FullAddress = "789 Đường Hai Bà Trưng, Phường Đa Kao, Quận 1, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận 1",
                            Ward = "Phường Đa Kao",
                            StreetAddress = "789 Đường Hai Bà Trưng",
                            Latitude = 10.7881m,
                            Longitude = 106.6983m,
                            RecipientPhone = "0912345678",
                            IsDefault = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new CustomerAddress
                        {
                            CustomerId = customer2.Id,
                            FullAddress = "321 Đường Võ Văn Tần, Phường 5, Quận 3, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận 3",
                            Ward = "Phường 5",
                            StreetAddress = "321 Đường Võ Văn Tần",
                            Latitude = 10.7821m,
                            Longitude = 106.6879m,
                            RecipientPhone = "0912345678",
                            IsDefault = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new CustomerAddress
                        {
                            CustomerId = customer3.Id,
                            FullAddress = "555 Đường Cách Mạng Tháng 8, Phường 11, Quận 3, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận 3",
                            Ward = "Phường 11",
                            StreetAddress = "555 Đường Cách Mạng Tháng 8",
                            Latitude = 10.7844m,
                            Longitude = 106.6759m,
                            RecipientPhone = "0901234567",
                            IsDefault = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new CustomerAddress
                        {
                            CustomerId = customer3.Id,
                            FullAddress = "888 Đường Lý Thường Kiệt, Phường 7, Quận Tân Bình, TP.HCM",
                            City = "Hồ Chí Minh",
                            District = "Quận Tân Bình",
                            Ward = "Phường 7",
                            StreetAddress = "888 Đường Lý Thường Kiệt",
                            Latitude = 10.7991m,
                            Longitude = 106.6532m,
                            RecipientPhone = "0901234567",
                            IsDefault = false,
                            IsActive = false,
                            CreatedAt = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding Areas
            if (!context.Areas.Any())
            {
                await context.Areas.AddRangeAsync(
                    new Area
                    {
                        AreaName = "Khu Nuôi Cá Bột A",
                        Description = "Khu vực khởi tạo quy trình nuôi, chuyên cho cá bột."
                    },
                    new Area
                    {
                        AreaName = "Khu Nuôi Cá Giống B",
                        Description = "Khu vực nuôi cá giống, giai đoạn trung gian."
                    },
                    new Area
                    {
                        AreaName = "Khu Nuôi Cá Hậu Bị C",
                        Description = "Khu vực nuôi cá chuẩn bị sinh sản hoặc thương phẩm."
                    },
                    new Area
                    {
                        AreaName = "Khu Cách Ly D",
                        Description = "Khu vực cách ly cho cá mới hoặc bị bệnh."
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Varieties
            if (!context.Varieties.Any())
            {
                await context.Varieties.AddRangeAsync(
                   new Variety
                   {
                       VarietyName = "Kohaku",
                       Characteristic = "Nền trắng với các mảng đỏ (Hi)",
                       OriginCountry = "Nhật Bản"
                   },
                   new Variety
                   {
                       VarietyName = "Sanke",
                       Characteristic = "Nền trắng, mảng đỏ và chấm đen (Sumi)",
                       OriginCountry = "Nhật Bản"
                   },
                   new Variety
                   {
                       VarietyName = "Showa",
                       Characteristic = "Nền đen, mảng trắng và mảng đỏ",
                       OriginCountry = "Nhật Bản"
                   },
                   new Variety
                   {
                       VarietyName = "Ogon",
                       Characteristic = "Màu đơn sắc ánh kim (vàng hoặc bạch kim)",
                       OriginCountry = "Nhật Bản"
                   },
                   new Variety
                   {
                       VarietyName = "Asagi",
                       Characteristic = "Lưng màu xanh/xám, vảy xếp lưới, bụng đỏ",
                       OriginCountry = "Nhật Bản"
                   }
               );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding PondTypes
            if (!context.PondTypes.Any())
            {
                await context.PondTypes.AddRangeAsync(
                    new PondType
                    {
                        TypeName = "Ao sinh sản",
                        Description = "Ao dành cho cá bố mẹ sinh sản, thường có giá thể để cá đẻ trứng.",
                        Type = TypeOfPond.Paring,
                        RecommendedQuantity = 2
                    },
                     new PondType
                     {
                         TypeName = "Ao ấp trứng",
                         Description = "Ao mô phỏng môi trường tự nhiên, có nhiều cây thủy sinh, bùn đáy, ít can thiệp kỹ thuật, dùng để thư giãn.",
                         Type = TypeOfPond.EggBatch,
                         RecommendedQuantity = 8000
                     },
                    new PondType
                    {
                        TypeName = "Ao ương cá bột",
                        Description = "Ao dùng để ương nuôi cá con mới nở (cá bột), cần nước sạch và thức ăn phù hợp.",
                        Type = TypeOfPond.FryFish,
                        RecommendedQuantity = 5000
                    },
                    new PondType
                    {
                        TypeName = "Ao trưng bày",
                        Description = "Ao được thiết kế đẹp mắt, dễ quan sát từ trên cao, ít cây thủy sinh, chú trọng thẩm mỹ.",
                        Type = TypeOfPond.Classification,
                        RecommendedQuantity = 2500
                    },
                    new PondType
                    {
                        TypeName = "Ao nuôi lớn",
                        Description = "Ao nuôi cá non (tosai) hoặc cá cần tăng trưởng nhanh, yêu cầu hệ thống lọc mạnh và chất lượng nước tốt.",
                        Type = TypeOfPond.BroodStock,
                        RecommendedQuantity = 5000
                    },
                    new PondType
                    {
                        TypeName = "Bể cách ly",
                        Description = "Bể nhỏ dùng để cách ly cá mới mua hoặc cá bệnh, cần kiểm soát nhiệt độ và khử trùng nghiêm ngặt.",
                        Type = TypeOfPond.MarketPond,
                        RecommendedQuantity = 150
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Ponds
            if (!context.Ponds.Any())
            {
                // Fetch actual Area IDs from database
                var areaA = await context.Areas.FirstOrDefaultAsync(a => a.AreaName == "Khu Nuôi Cá Bột A");
                var areaB = await context.Areas.FirstOrDefaultAsync(a => a.AreaName == "Khu Nuôi Cá Giống B");
                var areaC = await context.Areas.FirstOrDefaultAsync(a => a.AreaName == "Khu Nuôi Cá Hậu Bị C");
                var areaD = await context.Areas.FirstOrDefaultAsync(a => a.AreaName == "Khu Cách Ly D");

                if (areaA == null || areaB == null || areaC == null || areaD == null)
                {
                    throw new Exception("Areas must be seeded before Ponds");
                }

                await context.Ponds.AddRangeAsync(

                    // ===== 1. Ao sinh sản (PondTypeId = 1) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaC.Id,
                        PondTypeId = 1,
                        PondName = "Ao Sinh Sản SS-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu C - Góc Bắc",
                        LengthMeters = 3.5,
                        WidthMeters = 2.5,
                        DepthMeters = 1.0,
                        CapacityLiters = 8750,               // 3.5 × 2.5 × 1.0 × 1000
                        CurrentCapacity = 8200,              // ~94%
                        MaxFishCount = 4,
                        CurrentCount = 2,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaC.Id,
                        PondTypeId = 1,
                        PondName = "Ao Sinh Sản SS-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu C - Góc Nam",
                        LengthMeters = 3.5,
                        WidthMeters = 2.5,
                        DepthMeters = 1.0,
                        CapacityLiters = 8750,
                        CurrentCapacity = 8000,              // ~91%
                        MaxFishCount = 4,
                        CurrentCount = 3,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaC.Id,
                        PondTypeId = 1,
                        PondName = "Ao Sinh Sản SS-03",
                        PondStatus = PondStatus.Empty,
                        Location = "Khu C - Hàng giữa",
                        LengthMeters = 3.5,
                        WidthMeters = 2.5,
                        DepthMeters = 1.0,
                        CapacityLiters = 8750,
                        CurrentCapacity = 0,
                        MaxFishCount = 4,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaC.Id,
                        PondTypeId = 1,
                        PondName = "Ao Sinh Sản SS-04",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu C - Góc Tây",
                        LengthMeters = 3.5,
                        WidthMeters = 2.5,
                        DepthMeters = 1.0,
                        CapacityLiters = 8750,
                        CurrentCapacity = 3500,              // ~40%
                        MaxFishCount = 4,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },



                    // ===== 2. Ao ấp trứng (PondTypeId = 2) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 2,
                        PondName = "Ao Ấp Trứng AT-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu X - Phòng ấp",
                        LengthMeters = 5.0,
                        WidthMeters = 2.5,
                        DepthMeters = 1.5,
                        CapacityLiters = 18750,              // 5.0 × 2.5 × 1.5 × 1000
                        CurrentCapacity = 17500,             // ~93%
                        MaxFishCount = 50000,
                        CurrentCount = 30000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 2,
                        PondName = "Ao Ấp Trứng AT-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu X - Hàng 1",
                        LengthMeters = 5.0,
                        WidthMeters = 2.5,
                        DepthMeters = 1.5,
                        CapacityLiters = 18750,
                        CurrentCapacity = 17000,             // ~91%
                        MaxFishCount = 50000,
                        CurrentCount = 25000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 2,
                        PondName = "Ao Ấp Trứng AT-03",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu X - Hàng 2",
                        LengthMeters = 5.0,
                        WidthMeters = 2.5,
                        DepthMeters = 1.5,
                        CapacityLiters = 18750,
                        CurrentCapacity = 7500,              // ~40%
                        MaxFishCount = 50000,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 2,
                        PondName = "Ao Ấp Trứng AT-04",
                        PondStatus = PondStatus.Empty,
                        Location = "Khu X - Góc",
                        LengthMeters = 5.0,
                        WidthMeters = 2.5,
                        DepthMeters = 1.5,
                        CapacityLiters = 18750,
                        CurrentCapacity = 0,
                        MaxFishCount = 50000,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },



                    // ===== 3. Ao ương cá bột (PondTypeId = 3) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaA.Id,
                        PondTypeId = 3,
                        PondName = "Ao Ương Bột UB-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu A - Dãy 1",
                        LengthMeters = 4.0,
                        WidthMeters = 2.0,
                        DepthMeters = 1.2,
                        CapacityLiters = 9600,               // 4.0 × 2.0 × 1.2 × 1000
                        CurrentCapacity = 9000,              // ~94%
                        MaxFishCount = 20000,
                        CurrentCount = 15000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaA.Id,
                        PondTypeId = 3,
                        PondName = "Ao Ương Bột UB-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu A - Dãy 2",
                        LengthMeters = 4.0,
                        WidthMeters = 2.0,
                        DepthMeters = 1.2,
                        CapacityLiters = 9600,
                        CurrentCapacity = 8800,              // ~92%
                        MaxFishCount = 20000,
                        CurrentCount = 12000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaA.Id,
                        PondTypeId = 3,
                        PondName = "Ao Ương Bột UB-03",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu A - Dãy 3",
                        LengthMeters = 4.0,
                        WidthMeters = 2.0,
                        DepthMeters = 1.2,
                        CapacityLiters = 9600,
                        CurrentCapacity = 4000,              // ~42%
                        MaxFishCount = 20000,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaA.Id,
                        PondTypeId = 3,
                        PondName = "Ao Ương Bột UB-04",
                        PondStatus = PondStatus.Active,
                        Location = "Khu A - Dãy 4",
                        LengthMeters = 4.0,
                        WidthMeters = 2.0,
                        DepthMeters = 1.2,
                        CapacityLiters = 9600,
                        CurrentCapacity = 9200,              // ~96%
                        MaxFishCount = 20000,
                        CurrentCount = 18000,
                        CreatedAt = DateTime.UtcNow
                    },



                    // ===== 4. Ao trưng bày (PondTypeId = 4) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 4,
                        PondName = "Ao Trưng Bày TB-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu X - Khu khách",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 1.6,
                        CapacityLiters = 64000,              // 8.0 × 5.0 × 1.6 × 1000
                        CurrentCapacity = 60000,             // ~94%
                        MaxFishCount = 80,
                        CurrentCount = 65,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 4,
                        PondName = "Ao Trưng Bày TB-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu X - Góc vườn",
                        LengthMeters = 7.0,
                        WidthMeters = 5.0,
                        DepthMeters = 1.5,
                        CapacityLiters = 52500,              // 7.0 × 5.0 × 1.5 × 1000
                        CurrentCapacity = 49000,             // ~93%
                        MaxFishCount = 70,
                        CurrentCount = 55,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 4,
                        PondName = "Ao Trưng Bày TB-03",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu X - Sân chính",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 1.6,
                        CapacityLiters = 64000,
                        CurrentCapacity = 25000,             // ~39%
                        MaxFishCount = 80,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaB.Id,
                        PondTypeId = 4,
                        PondName = "Ao Trưng Bày TB-04",
                        PondStatus = PondStatus.Active,
                        Location = "Khu B - Góc đẹp",
                        LengthMeters = 6.0,
                        WidthMeters = 4.0,
                        DepthMeters = 1.4,
                        CapacityLiters = 33600,              // 6.0 × 4.0 × 1.4 × 1000
                        CurrentCapacity = 31000,             // ~92%
                        MaxFishCount = 60,
                        CurrentCount = 48,
                        CreatedAt = DateTime.UtcNow
                    },



                    // ===== 5. Ao nuôi lớn (PondTypeId = 5) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaB.Id,
                        PondTypeId = 5,
                        PondName = "Ao Nuôi Lớn NL-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu B - Dãy chính",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 2.0,
                        CapacityLiters = 80000,              // 8.0 × 5.0 × 2.0 × 1000
                        CurrentCapacity = 75000,             // ~94%
                        MaxFishCount = 4000,
                        CurrentCount = 3200,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaB.Id,
                        PondTypeId = 5,
                        PondName = "Ao Nuôi Lớn NL-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu B - Dãy phụ",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 2.0,
                        CapacityLiters = 80000,
                        CurrentCapacity = 72000,             // ~90%
                        MaxFishCount = 4000,
                        CurrentCount = 2800,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaB.Id,
                        PondTypeId = 5,
                        PondName = "Ao Nuôi Lớn NL-03",
                        PondStatus = PondStatus.Empty,
                        Location = "Khu B - Góc",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 2.0,
                        CapacityLiters = 80000,
                        CurrentCapacity = 0,
                        MaxFishCount = 4000,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaB.Id,
                        PondTypeId = 5,
                        PondName = "Ao Nuôi Lớn NL-04",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu B - Hàng cuối",
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        DepthMeters = 2.0,
                        CapacityLiters = 80000,
                        CurrentCapacity = 30000,             // ~38%
                        MaxFishCount = 4000,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },



                    // ===== 6. Bể cách ly (PondTypeId = 6) – 4 hồ =====
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 6,
                        PondName = "Bể Cách Ly CL-01",
                        PondStatus = PondStatus.Active,
                        Location = "Khu D - Phòng 1",
                        LengthMeters = 1.2,
                        WidthMeters = 1.0,
                        DepthMeters = 0.8,
                        CapacityLiters = 960,                // 1.2 × 1.0 × 0.8 × 1000
                        CurrentCapacity = 850,               // ~89%
                        MaxFishCount = 30,
                        CurrentCount = 12,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 6,
                        PondName = "Bể Cách Ly CL-02",
                        PondStatus = PondStatus.Active,
                        Location = "Khu D - Phòng 2",
                        LengthMeters = 1.2,
                        WidthMeters = 1.0,
                        DepthMeters = 0.8,
                        CapacityLiters = 960,
                        CurrentCapacity = 800,               // ~83%
                        MaxFishCount = 30,
                        CurrentCount = 8,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 6,
                        PondName = "Bể Cách Ly CL-03",
                        PondStatus = PondStatus.Empty,
                        Location = "Khu D - Phòng 3",
                        LengthMeters = 1.2,
                        WidthMeters = 1.0,
                        DepthMeters = 0.8,
                        CapacityLiters = 960,
                        CurrentCapacity = 0,
                        MaxFishCount = 30,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = areaD.Id,
                        PondTypeId = 6,
                        PondName = "Bể Cách Ly CL-04",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Khu D - Phòng 4",
                        LengthMeters = 1.2,
                        WidthMeters = 1.0,
                        DepthMeters = 0.8,
                        CapacityLiters = 960,
                        CurrentCapacity = 400,               // ~42%
                        MaxFishCount = 30,
                        CurrentCount = 0,
                        CreatedAt = DateTime.UtcNow
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding KoiFishes
            if (!context.KoiFishes.Any())
            {
                await context.KoiFishes.AddRangeAsync(
                    // ================== KOHAKU (1) - 6 con ==================
                    // KOI-0001 - Kohaku Male - Maruten
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 5, 10),
                        Description = "Kohaku đực Maruten, Hi đỏ tròn trên đầu, thân thon dài, nền trắng tuyết.",
                        Gender = Gender.Male,
                        Pattern = "Maruten",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://www.nishikigoi-export.jp/wp-content/uploads/2024/02/240212-035_01-scaled.jpg" },
                        PondId = 1,
                        RFID = "KOI-0001",
                        SellingPrice = 65000000m,
                        Size = 28.5,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0002 - Kohaku Female - Tancho
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 5, 10),
                        Description = "Kohaku mái Tancho, chấm đỏ tròn trên đầu, thân bầu, sinh sản tốt.",
                        Gender = Gender.Female,
                        Pattern = "Tancho",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2023/01/tancho-kohaku.jpg" },
                        PondId = 1,
                        RFID = "KOI-0002",
                        SellingPrice = 62000000m,
                        Size = 29.0,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0003 - Kohaku Male - Inazuma
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 4, 18),
                        Description = "Kohaku đực Inazuma, Hi đỏ tia chớp chạy dọc thân, nền trắng tinh.",
                        Gender = Gender.Male,
                        Pattern = "Inazuma",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/05/kohaku-inazuma.jpg" },
                        PondId = 2,
                        RFID = "KOI-0003",
                        SellingPrice = 58000000m,
                        Size = 29.0,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // KOI-0004 - Kohaku Female - Nidan
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 7, 22),
                        Description = "Kohaku mái Nidan, 2 mảng Hi đỏ cân đối, thân bầu, nền trắng sạch.",
                        Gender = Gender.Female,
                        Pattern = "Nidan",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2023/07/kohaku-nidan.jpg" },
                        PondId = 3,
                        RFID = "KOI-0004",
                        SellingPrice = 61000000m,
                        Size = 30.0,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0005 - Kohaku Male - Sandan
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 6, 12),
                        Description = "Kohaku đực Sandan, 3 mảng Hi đỏ đều, thân dài, dáng chuẩn show.",
                        Gender = Gender.Male,
                        Pattern = "Sandan",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2022/08/kohaku-sandan.jpg" },
                        PondId = 4,
                        RFID = "KOI-0005",
                        SellingPrice = 67000000m,
                        Size = 30.5,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Hi đỏ đậm"
                    },

                    // KOI-0006 - Kohaku Female - Kuchibeni
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 8, 10),
                        Description = "Kohaku mái Kuchibeni, môi đỏ nổi bật, Hi đều, thân bầu sinh sản.",
                        Gender = Gender.Female,
                        Pattern = "Kuchibeni",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/01/kohaku-kuchibeni.jpg" },
                        PondId = 5,
                        RFID = "KOI-0006",
                        SellingPrice = 70000000m,
                        Size = 31.0,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // ================== SANKE (2) - 6 con ==================
                    // KOI-0007 - Sanke Male - Inazuma
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 8, 25),
                        Description = "Sanke đực Inazuma, Sumi đen tia chớp, Hi đỏ tươi, nền trắng sạch.",
                        Gender = Gender.Male,
                        Pattern = "Inazuma",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://danviet.ex-cdn.com/files/f1/296231569849192448/2022/5/10/ca-koi-dat-nhat-the-gioijpg2-1652174894796.jpg" },
                        PondId = 2,
                        RFID = "KOI-0007",
                        SellingPrice = 48000000m,
                        Size = 27.2,
                        Type = KoiType.High,
                        VarietyId = 2,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ginrin nhẹ"
                    },

                    // KOI-0008 - Sanke Female - Tsubo Sumi
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 7, 1),
                        Description = "Sanke mái Tsubo Sumi, chấm đen tròn đều, Hi đỏ rực, gen ổn định.",
                        Gender = Gender.Female,
                        Pattern = "Tsubo Sumi",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/04/sanke-tsubo.jpg" },
                        PondId = 3,
                        RFID = "KOI-0008",
                        SellingPrice = 45000000m,
                        Size = 28.5,
                        Type = KoiType.High,
                        VarietyId = 2,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ginrin toàn thân"
                    },

                    // KOI-0009 - Sanke Male - Tsubo Sumi
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 10, 5),
                        Description = "Sanke đực Tsubo Sumi, chấm đen tròn đều, Hi đỏ đậm, dáng cân đối.",
                        Gender = Gender.Male,
                        Pattern = "Tsubo Sumi",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2023/06/sanke-tsubo.jpg" },
                        PondId = 4,
                        RFID = "KOI-0009",
                        SellingPrice = 72000000m,
                        Size = 31.5,
                        Type = KoiType.Show,
                        VarietyId = 2,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // KOI-0010 - Sanke Female - Maruten
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 3, 30),
                        Description = "Sanke mái Maruten, Sumi chấm đầu, Hi đỏ tròn, nền trắng sạch.",
                        Gender = Gender.Female,
                        Pattern = "Maruten",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/04/sanke-maruten.jpg" },
                        PondId = 5,
                        RFID = "KOI-0010",
                        SellingPrice = 49000000m,
                        Size = 28.0,
                        Type = KoiType.High,
                        VarietyId = 2,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ginrin nhẹ"
                    },

                    // KOI-0011 - Sanke Male - Ginrin
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 1, 10),
                        Description = "Sanke đực Ginrin toàn thân, Sumi đen nổi, Hi đỏ rực, ánh kim mạnh.",
                        Gender = Gender.Male,
                        Pattern = "Ginrin",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/02/sanke-ginrin.jpg" },
                        PondId = 1,
                        RFID = "KOI-0011",
                        SellingPrice = 81000000m,
                        Size = 32.0,
                        Type = KoiType.Show,
                        VarietyId = 2,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ginrin mạnh"
                    },

                    // KOI-0012 - Sanke Female - Young
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 2, 20),
                        Description = "Sanke mái non, Sumi đang lên, Hi đỏ tươi, tiềm năng cao.",
                        Gender = Gender.Female,
                        Pattern = "Standard",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2024/01/sanke-young.jpg" },
                        PondId = 2,
                        RFID = "KOI-0012",
                        SellingPrice = 25000000m,
                        Size = 23.0,
                        Type = KoiType.High,
                        VarietyId = 2,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // ================== SHOWA (3) - 6 con ==================
                    // KOI-0013 - Showa Male
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 9, 15),
                        Description = "Showa đực mạnh mẽ, Sumi đen tuyền, Hi đỏ sâu, Moto-guro đầu.",
                        Gender = Gender.Male,
                        Pattern = "Standard",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://www.kodamakoifarm.com/wp-content/uploads/2025/09/x0910s045.jpg" },
                        PondId = 3,
                        RFID = "KOI-0013",
                        SellingPrice = 78000000m,
                        Size = 30.5,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0014 - Showa Female
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 8, 5),
                        Description = "Showa mái dòng Dainichi, Sumi đen bao nền, Hi đỏ nổi, mẹ giống tốt.",
                        Gender = Gender.Female,
                        Pattern = "Hi Showa",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/02/hi-showa.jpg" },
                        PondId = 4,
                        RFID = "KOI-0014",
                        SellingPrice = 88000000m,
                        Size = 33.0,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0015 - Showa Male - Kindai
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 11, 20),
                        Description = "Showa đực Kindai, Sumi bao nền, ít Shiroji, Hi đỏ nổi bật.",
                        Gender = Gender.Male,
                        Pattern = "Kindai",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2024/01/showa-kindai.jpg" },
                        PondId = 5,
                        RFID = "KOI-0015",
                        SellingPrice = 95000000m,
                        Size = 34.0,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Marudo Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Sumi siêu đậm"
                    },

                    // KOI-0016 - Showa Female - Hi Showa
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 12, 1),
                        Description = "Hi Showa mái, đỏ chiếm ưu thế, Sumi viền rõ, Shiroji sáng.",
                        Gender = Gender.Female,
                        Pattern = "Hi Showa",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/02/hi-showa-female.jpg" },
                        PondId = 1,
                        RFID = "KOI-0016",
                        SellingPrice = 92000000m,
                        Size = 35.0,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Isa Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // KOI-0017 - Showa Male - Boke
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 10, 5),
                        Description = "Showa đực Boke, Sumi mờ nghệ thuật, Hi đỏ sâu, dáng mạnh.",
                        Gender = Gender.Male,
                        Pattern = "Boke",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/09/showa-boke.jpg" },
                        PondId = 2,
                        RFID = "KOI-0017",
                        SellingPrice = 89000000m,
                        Size = 33.5,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Boke tự nhiên"
                    },

                    // KOI-0018 - Showa Female - Modern
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 3, 15),
                        Description = "Showa mái Modern, Sumi đậm, Hi đỏ lớn, dáng mạnh, dòng mới.",
                        Gender = Gender.Female,
                        Pattern = "Modern",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/03/showa-modern.jpg" },
                        PondId = 3,
                        RFID = "KOI-0018",
                        SellingPrice = 98000000m,
                        Size = 36.0,
                        Type = KoiType.Show,
                        VarietyId = 3,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // ================== OGON (4) - 6 con ==================
                    // KOI-0019 - Ogon Male - Yamabuki
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 6, 10),
                        Description = "Yamabuki Ogon đực, vàng kim rực rỡ, thân to khỏe, ánh kim đều.",
                        Gender = Gender.Male,
                        Pattern = "Solid",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string> { "https://cdn0497.cdn4s.com/media/fish/img_0095.png" },
                        PondId = 4,
                        RFID = "KOI-0019",
                        SellingPrice = 85000000m,
                        Size = 32.0,
                        Type = KoiType.Show,
                        VarietyId = 4,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ánh kim vàng mạnh"
                    },

                    // KOI-0020 - Ogon Female - Yamabuki
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 11, 20),
                        Description = "Yamabuki Ogon mái, vàng kim óng ánh, thân tròn, ánh kim đều.",
                        Gender = Gender.Female,
                        Pattern = "Solid",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/08/ogon-yamabuki.jpg" },
                        PondId = 5,
                        RFID = "KOI-0020",
                        SellingPrice = 52000000m,
                        Size = 30.0,
                        Type = KoiType.Show,
                        VarietyId = 4,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ánh kim vàng đậm"
                    },

                    // KOI-0021 - Ogon Male - Platinum
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 1, 15),
                        Description = "Platinum Ogon đực, trắng bạc ánh kim, không tỳ vết, dáng đẹp.",
                        Gender = Gender.Male,
                        Pattern = "Solid",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/01/platinum-ogon.jpg" },
                        PondId = 1,
                        RFID = "KOI-0021",
                        SellingPrice = 68000000m,
                        Size = 30.5,
                        Type = KoiType.Show,
                        VarietyId = 4,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ánh kim bạch kim"
                    },

                    // KOI-0022 - Ogon Female - Cream
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 9, 15),
                        Description = "Cream Ogon mái, vàng kem óng ánh, thân tròn, ánh kim đều.",
                        Gender = Gender.Female,
                        Pattern = "Solid",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/07/cream-ogon.jpg" },
                        PondId = 2,
                        RFID = "KOI-0022",
                        SellingPrice = 55000000m,
                        Size = 31.0,
                        Type = KoiType.Show,
                        VarietyId = 4,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ánh kim kem"
                    },

                    // KOI-0023 - Ogon Male - Hariwake
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 2, 28),
                        Description = "Hariwake đực (Ogon lai), vàng kim trên nền trắng ánh kim.",
                        Gender = Gender.Male,
                        Pattern = "Hariwake",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2023/09/hariwake.jpg" },
                        PondId = 3,
                        RFID = "KOI-0023",
                        SellingPrice = 48000000m,
                        Size = 29.0,
                        Type = KoiType.High,
                        VarietyId = 4,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Ogon lai"
                    },

                    // KOI-0024 - Ogon Female - Young
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 3, 10),
                        Description = "Yamabuki Ogon mái non, vàng kim sáng, tiềm năng cao.",
                        Gender = Gender.Female,
                        Pattern = "Solid",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2024/02/ogon-young.jpg" },
                        PondId = 4,
                        RFID = "KOI-0024",
                        SellingPrice = 32000000m,
                        Size = 25.0,
                        Type = KoiType.High,
                        VarietyId = 4,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"                  
                    },

                    // ================== ASAGI (5) - 6 con ==================
                    // KOI-0025 - Asagi Male
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 3, 12),
                        Description = "Asagi đực, vảy lưới xanh cobalt, bụng Hi đỏ cam, viền trắng rõ.",
                        Gender = Gender.Male,
                        Pattern = "Standard",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://cdn0497.cdn4s.com/media/fish/img_0119.png" },
                        PondId = 5,
                        RFID = "KOI-0025",
                        SellingPrice = 22000000m,
                        Size = 24.0,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"  
                    },

                    // KOI-0026 - Asagi Female
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 2, 15),
                        Description = "Asagi mái, vảy xanh bạc đều, bụng Hi đỏ, gen ổn định.",
                        Gender = Gender.Female,
                        Pattern = "Standard",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/06/asagi-female.jpg" },
                        PondId = 1,
                        RFID = "KOI-0026",
                        SellingPrice = 33000000m,
                        Size = 27.5,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có",
                         
                    },

                    // KOI-0027 - Asagi Male - Mizuho
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 6, 10),
                        Description = "Asagi đực Mizuho, xanh ánh tím, viền trắng sắc, bụng đỏ cam.",
                        Gender = Gender.Male,
                        Pattern = "Mizuho",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2023/07/asagi-mizuho.jpg" },
                        PondId = 2,
                        RFID = "KOI-0027",
                        SellingPrice = 28000000m,
                        Size = 26.5,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                    },

                    // KOI-0028 - Asagi Female - Konjo
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 5, 20),
                        Description = "Asagi mái Konjo, xanh đậm ánh tím, vảy đều, bụng đỏ rực.",
                        Gender = Gender.Female,
                        Pattern = "Konjo",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/07/asagi-konjo.jpg" },
                        PondId = 3,
                        RFID = "KOI-0028",
                        SellingPrice = 35000000m,
                        Size = 27.0,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0029 - Asagi Male - Young
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 1, 15),
                        Description = "Asagi đực non, vảy xanh bắt đầu rõ, tiềm năng cao, giá tốt.",
                        Gender = Gender.Male,
                        Pattern = "Standard",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://kodamakoifarm.com/wp-content/uploads/2024/01/asagi-young.jpg" },
                        PondId = 4,
                        RFID = "KOI-0029",
                        SellingPrice = 18000000m,
                        Size = 22.0,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationDescription = "Không có"
                         
                    },

                    // KOI-0030 - Asagi Female - Hi Asagi
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 7, 5),
                        Description = "Asagi mái Hi Asagi, bụng đỏ cam rực, vảy xanh đậm, gen tốt.",
                        Gender = Gender.Female,
                        Pattern = "Hi Asagi",
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string> { "https://nishikigoi.life/wp-content/uploads/2023/08/hi-asagi.jpg" },
                        PondId = 5,
                        RFID = "KOI-0030",
                        SellingPrice = 39000000m,
                        Size = 28.5,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationDescription = "Hi đỏ mạnh"
                    }
                );

                await context.SaveChangesAsync();
            }

            #endregion

            #region Seeding KoiFavorites
            if (!context.KoiFavorites.Any() && customer1 != null && customer2 != null && customer3 != null)
            {
                var koiFishes = await context.KoiFishes.OrderBy(k => k.Id).Take(20).ToListAsync();

                if (koiFishes.Count >= 10)
                {
                    await context.KoiFavorites.AddRangeAsync(
                        // Customer1 favorites
                        new KoiFavorite
                        {
                            UserId = customer1.Id,
                            KoiFishId = koiFishes[0].Id, // KOI-0001
                            CreatedAt = DateTime.UtcNow.AddDays(-10)
                        },
                        new KoiFavorite
                        {
                            UserId = customer1.Id,
                            KoiFishId = koiFishes[2].Id, // KOI-0003
                            CreatedAt = DateTime.UtcNow.AddDays(-8)
                        },
                        new KoiFavorite
                        {
                            UserId = customer1.Id,
                            KoiFishId = koiFishes[5].Id, // KOI-0006
                            CreatedAt = DateTime.UtcNow.AddDays(-5)
                        },
                        new KoiFavorite
                        {
                            UserId = customer1.Id,
                            KoiFishId = koiFishes[7].Id, // KOI-0008
                            CreatedAt = DateTime.UtcNow.AddDays(-3)
                        },

                        // Customer2 favorites
                        new KoiFavorite
                        {
                            UserId = customer2.Id,
                            KoiFishId = koiFishes[1].Id, // KOI-0002
                            CreatedAt = DateTime.UtcNow.AddDays(-12)
                        },
                        new KoiFavorite
                        {
                            UserId = customer2.Id,
                            KoiFishId = koiFishes[4].Id, // KOI-0005
                            CreatedAt = DateTime.UtcNow.AddDays(-7)
                        },
                        new KoiFavorite
                        {
                            UserId = customer2.Id,
                            KoiFishId = koiFishes[6].Id, // KOI-0007
                            CreatedAt = DateTime.UtcNow.AddDays(-4)
                        },
                        new KoiFavorite
                        {
                            UserId = customer2.Id,
                            KoiFishId = koiFishes[9].Id, // KOI-0010
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        },

                        // Customer3 favorites
                        new KoiFavorite
                        {
                            UserId = customer3.Id,
                            KoiFishId = koiFishes[0].Id, // KOI-0001 (same as customer1)
                            CreatedAt = DateTime.UtcNow.AddDays(-9)
                        },
                        new KoiFavorite
                        {
                            UserId = customer3.Id,
                            KoiFishId = koiFishes[3].Id, // KOI-0004
                            CreatedAt = DateTime.UtcNow.AddDays(-6)
                        },
                        new KoiFavorite
                        {
                            UserId = customer3.Id,
                            KoiFishId = koiFishes[8].Id, // KOI-0009
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }

            #endregion

            #region Seeding BreedingProcesses
            if (!context.BreedingProcesses.Any())
            {
                await context.BreedingProcesses.AddRangeAsync(
                    // ============================= 1. Pairing (2 bản) =============================
                    new BreedingProcess
                    {
                        Code = "BP-001",
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 1,
                        StartDate = DateTime.Now,
                        EndDate = null,
                        Status = BreedingStatus.Pairing,
                        Note = "Ghép cặp Kohaku đực x cái, đang theo dõi.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 0,
                        FertilizationRate = 0,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },
                    new BreedingProcess
                    {
                        Code = "BP-002",
                        MaleKoiId = 7,
                        FemaleKoiId = 8,
                        PondId = 2,
                        StartDate = DateTime.Now,
                        EndDate = null,
                        Status = BreedingStatus.Pairing,
                        Note = "Ghép cặp Sanke đực x cái, chờ phản ứng.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 0,
                        FertilizationRate = 0,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },

                    // ============================= 2. Spawned (2 bản) =============================
                    new BreedingProcess
                    {
                        Code = "BP-003",
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 1,
                        StartDate = DateTime.Now.AddDays(-3),
                        EndDate = DateTime.Now.AddDays(-3),
                        Status = BreedingStatus.Spawned,
                        Note = "Kohaku đã đẻ 1.500 trứng.",
                        Result = BreedingResult.PartialSuccess,
                        TotalEggs = 1500,
                        FertilizationRate = 0.85,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },
                    new BreedingProcess
                    {
                        Code = "BP-004",
                        MaleKoiId = 7,
                        FemaleKoiId = 8,
                        PondId = 2,
                        StartDate = DateTime.Now.AddDays(-4),
                        EndDate = DateTime.Now.AddDays(-4),
                        Status = BreedingStatus.Spawned,
                        Note = "Sanke đã đẻ 1.800 trứng.",
                        Result = BreedingResult.PartialSuccess,
                        TotalEggs = 1800,
                        FertilizationRate = 0.88,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },

                    // ============================= 3. EggBatch (2 bản) =============================
                    new BreedingProcess
                    {
                        Code = "BP-005",
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 5,
                        StartDate = DateTime.Now.AddDays(-7),
                        EndDate = null,
                        Status = BreedingStatus.EggBatch,
                        Note = "Trứng Kohaku đang ấp, tỷ lệ thụ tinh 90%.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 1500,
                        FertilizationRate = 0.90,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },
                    new BreedingProcess
                    {
                        Code = "BP-006",
                        MaleKoiId = 7,
                        FemaleKoiId = 8,
                        PondId = 6,
                        StartDate = DateTime.Now.AddDays(-8),
                        EndDate = null,
                        Status = BreedingStatus.EggBatch,
                        Note = "Trứng Sanke đang ấp, tỷ lệ thụ tinh 87%.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 1800,
                        FertilizationRate = 0.87,
                        HatchingRate = null,
                        SurvivalRate = 0,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },

                    // ============================= 4. FryFish (2 bản) =============================
                    new BreedingProcess
                    {
                        Code = "BP-007",
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 9,
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = null,
                        Status = BreedingStatus.FryFish,
                        Note = "Cá bột Kohaku đã nở, đang cho ăn Artemia.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 1500,
                        FertilizationRate = 0.90,
                        HatchingRate = 0.75,
                        SurvivalRate = 0.75,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },
                    new BreedingProcess
                    {
                        Code = "BP-008",
                        MaleKoiId = 7,
                        FemaleKoiId = 8,
                        PondId = 10,
                        StartDate = DateTime.Now.AddDays(-16),
                        EndDate = null,
                        Status = BreedingStatus.FryFish,
                        Note = "Cá bột Sanke đã nở, đang nuôi lớn.",
                        Result = BreedingResult.Unknown,
                        TotalEggs = 1800,
                        FertilizationRate = 0.87,
                        HatchingRate = 0.73,
                        SurvivalRate = 0.73,
                        TotalFishQualified = 0,
                        TotalPackage = 0
                    },

                    // ============================= 5. Classification (2 bản) =============================
                    new BreedingProcess
                    {
                        Code = "BP-009",
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 16,
                        StartDate = DateTime.Now.AddDays(-25),
                        EndDate = DateTime.Now.AddDays(-20),
                        Status = BreedingStatus.Classification,
                        Note = "Phân loại Kohaku: chọn 300 con đẹp nhất (size 3-5cm), đóng 3 bao.",
                        Result = BreedingResult.PartialSuccess,
                        TotalEggs = 1500,
                        FertilizationRate = 0.90,
                        HatchingRate = 0.75,
                        SurvivalRate = 0.70,
                        TotalFishQualified = 300,
                        TotalPackage = 3
                    },
                    new BreedingProcess
                    {
                        Code = "BP-010",
                        MaleKoiId = 7,
                        FemaleKoiId = 8,
                        PondId = 17,
                        StartDate = DateTime.Now.AddDays(-26),
                        EndDate = DateTime.Now.AddDays(-21),
                        Status = BreedingStatus.Classification,
                        Note = "Phân loại Sanke: chọn 350 con pattern rõ, đóng 4 bao.",
                        Result = BreedingResult.PartialSuccess,
                        TotalEggs = 1800,
                        FertilizationRate = 0.87,
                        HatchingRate = 0.73,
                        SurvivalRate = 0.68,
                        TotalFishQualified = 350,
                        TotalPackage = 4
                    },

                    // ============================= 6. QUY TRÌNH THÀNH CÔNG – BP-011 → BP-015 ĐỀU LÀ COMPLETE =============================
                    // BP-011: Complete – Ghép cặp thành công
                    new BreedingProcess
                    {
                        Code = "BP-011",
                        MaleKoiId = 13,
                        FemaleKoiId = 14,
                        PondId = 1,
                        StartDate = DateTime.Now.AddDays(-90),
                        EndDate = DateTime.Now.AddDays(-87),
                        Status = BreedingStatus.Complete,
                        Note = "Ghép cặp Showa đực x cái – dòng Dainichi. Thành công, chuyển sang sinh sản.",
                        Result = BreedingResult.Success,
                        TotalEggs = 4000,
                        FertilizationRate = 0.95,
                        HatchingRate = 0.88,
                        SurvivalRate = 0.88,
                        TotalFishQualified = 1200,
                        TotalPackage = 25
                    },

                    // BP-012: Complete – Đẻ trứng thành công
                    new BreedingProcess
                    {
                        Code = "BP-012",
                        MaleKoiId = 13,
                        FemaleKoiId = 14,
                        PondId = 1,
                        StartDate = DateTime.Now.AddDays(-87),
                        EndDate = DateTime.Now.AddDays(-87),
                        Status = BreedingStatus.Complete,
                        Note = "Showa đẻ 4.000 trứng chất lượng cao.",
                        Result = BreedingResult.Success,
                        TotalEggs = 4000,
                        FertilizationRate = 0.95,
                        HatchingRate = 0.88,
                        SurvivalRate = 0.88,
                        TotalFishQualified = 1200,
                        TotalPackage = 25
                    },

                    // BP-013: Complete – Ấp trứng thành công
                    new BreedingProcess
                    {
                        Code = "BP-013",
                        MaleKoiId = 13,
                        FemaleKoiId = 14,
                        PondId = 5,
                        StartDate = DateTime.Now.AddDays(-82),
                        EndDate = DateTime.Now.AddDays(-76),
                        Status = BreedingStatus.Complete,
                        Note = "Trứng Showa ấp tốt, tỷ lệ thụ tinh 95%, không nấm.",
                        Result = BreedingResult.Success,
                        TotalEggs = 4000,
                        FertilizationRate = 0.95,
                        HatchingRate = 0.88,
                        SurvivalRate = 0.88,
                        TotalFishQualified = 1200,
                        TotalPackage = 25
                    },

                    // BP-014: Complete – Nuôi bột thành công
                    new BreedingProcess
                    {
                        Code = "BP-014",
                        MaleKoiId = 13,
                        FemaleKoiId = 14,
                        PondId = 9,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40),
                        Status = BreedingStatus.Complete,
                        Note = "Cá bột Showa phát triển tốt, tỷ lệ sống 88%.",
                        Result = BreedingResult.Success,
                        TotalEggs = 4000,
                        FertilizationRate = 0.95,
                        HatchingRate = 0.88,
                        SurvivalRate = 0.88,
                        TotalFishQualified = 1200,
                        TotalPackage = 25
                    },

                    // BP-015: Complete – Toàn bộ quy trình hoàn tất
                    new BreedingProcess
                    {
                        Code = "BP-015",
                        MaleKoiId = 13,
                        FemaleKoiId = 14,
                        PondId = null,
                        StartDate = DateTime.Now.AddDays(-90),
                        EndDate = DateTime.Now.AddDays(-30),
                        Status = BreedingStatus.Complete,
                        Note = "HOÀN TẤT XUẤT SẮC! 1.200 con F1 Showa đạt chuẩn show",
                        Result = BreedingResult.Success,
                        TotalEggs = 4000,
                        FertilizationRate = 0.95,
                        HatchingRate = 0.88,
                        SurvivalRate = 0.88,
                        TotalFishQualified = 1200,
                        TotalPackage = 25
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding EggBatches
            if (!context.EggBatches.Any())
            {
                await context.EggBatches.AddRangeAsync(
                    // ============================= 1. BP-005: Kohaku - Đang ấp (EggBatch) =============================
                    new EggBatch
                    {
                        BreedingProcessId = 5,
                        Quantity = 1500,
                        TotalHatchedEggs = null, // Chưa nở
                        FertilizationRate = 0.90,
                        Status = EggBatchStatus.Incubating,
                        SpawnDate = DateTime.Now.AddDays(-7),
                        HatchingTime = DateTime.Now.AddDays(-7).AddDays(5),
                        EndDate = null
                    },

                    // ============================= 2. BP-006: Sanke - Đang ấp (EggBatch) =============================
                    new EggBatch
                    {
                        BreedingProcessId = 6,
                        Quantity = 1800,
                        TotalHatchedEggs = null,
                        FertilizationRate = 0.87,
                        Status = EggBatchStatus.Incubating,
                        SpawnDate = DateTime.Now.AddDays(-8),
                        HatchingTime = DateTime.Now.AddDays(-8).AddDays(6),
                        EndDate = null
                    },

                    // ============================= 3. BP-007: Kohaku - Đã nở (FryFish) =============================
                    new EggBatch
                    {
                        BreedingProcessId = 7,
                        Quantity = 1500,
                        TotalHatchedEggs = 1012, // 1500 × 0.90 × 0.75 (HatchingRate = 0.75)
                        FertilizationRate = 0.90,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-18),
                        HatchingTime = DateTime.Now.AddDays(-15),
                        EndDate = DateTime.Now.AddDays(-15)
                    },

                    // ============================= 4. BP-008: Sanke - Đã nở (FryFish) =============================
                    new EggBatch
                    {
                        BreedingProcessId = 8,
                        Quantity = 1800,
                        TotalHatchedEggs = 1143, // 1800 × 0.87 × 0.73
                        FertilizationRate = 0.87,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-19),
                        HatchingTime = DateTime.Now.AddDays(-16),
                        EndDate = DateTime.Now.AddDays(-16)
                    },

                   
                    new EggBatch
                    {
                        BreedingProcessId = 11,
                        Quantity = 4000,
                        TotalHatchedEggs = 3344, // 4000 × 0.95 × 0.88
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-90),
                        HatchingTime = DateTime.Now.AddDays(-84),
                        EndDate = DateTime.Now.AddDays(-84)
                    },

                 
                    new EggBatch
                    {
                        BreedingProcessId = 12,
                        Quantity = 4000,
                        TotalHatchedEggs = 3344,
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-87),
                        HatchingTime = DateTime.Now.AddDays(-81),
                        EndDate = DateTime.Now.AddDays(-81)
                    },

                  
                    new EggBatch
                    {
                        BreedingProcessId = 13,
                        Quantity = 4000,
                        TotalHatchedEggs = 3344,
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-82),
                        HatchingTime = DateTime.Now.AddDays(-76),
                        EndDate = DateTime.Now.AddDays(-76)
                    },

                  
                    new EggBatch
                    {
                        BreedingProcessId = 14,
                        Quantity = 4000,
                        TotalHatchedEggs = 3344,
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-70),
                        HatchingTime = DateTime.Now.AddDays(-64),
                        EndDate = DateTime.Now.AddDays(-64)
                    },

                    new EggBatch
                    {
                        BreedingProcessId = 15,
                        Quantity = 4000,
                        TotalHatchedEggs = 3344,
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-90),
                        HatchingTime = DateTime.Now.AddDays(-84),
                        EndDate = DateTime.Now.AddDays(-84)
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding IncubationDailyRecords
            if (!context.IncubationDailyRecords.Any())
            {
                await context.IncubationDailyRecords.AddRangeAsync(
                    // ===================================================================
                    // 1. EggBatchId = 1 (BP-005: Kohaku - Đang ấp) → 3 ngày → TotalEggs = 1500
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 1,
                        DayNumber = DateTime.Now.AddDays(-7),
                        HealthyEggs = 1350,
                        RottenEggs = 150,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-7)
                        // 1350 + 150 + 0 = 1500
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 1,
                        DayNumber = DateTime.Now.AddDays(-6),
                        HealthyEggs = 1320,
                        RottenEggs = 180,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-6)
                        // 1320 + 180 + 0 = 1500
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 1,
                        DayNumber = DateTime.Now.AddDays(-5),
                        HealthyEggs = 1300,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-5)
                        // 1300 + 200 + 0 = 1500
                    },

                    // ===================================================================
                    // 2. EggBatchId = 2 (BP-006: Sanke - Đang ấp) → 2 ngày → TotalEggs = 1800
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 2,
                        DayNumber = DateTime.Now.AddDays(-8),
                        HealthyEggs = 1566,
                        RottenEggs = 234,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-8)
                        // 1566 + 234 + 0 = 1800
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 2,
                        DayNumber = DateTime.Now.AddDays(-7),
                        HealthyEggs = 1530,
                        RottenEggs = 270,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-7)
                        // 1530 + 270 + 0 = 1800
                    },

                    // ===================================================================
                    // 3. EggBatchId = 3 (BP-007: Kohaku - Đã nở) → 4 ngày → TotalEggs = 1500
                    //    → FryFish InitialCount = 1125
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 3,
                        DayNumber = DateTime.Now.AddDays(-18),
                        HealthyEggs = 1350,
                        RottenEggs = 150,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-18)
                        // 1350 + 150 + 0 = 1500
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 3,
                        DayNumber = DateTime.Now.AddDays(-17),
                        HealthyEggs = 1200,
                        RottenEggs = 200,
                        HatchedEggs = 100,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-17)
                        // 1200 + 200 + 100 = 1500
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 3,
                        DayNumber = DateTime.Now.AddDays(-16),
                        HealthyEggs = 675,
                        RottenEggs = 300,
                        HatchedEggs = 525,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-16)
                        // 675 + 300 + 525 = 1500
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 3,
                        DayNumber = DateTime.Now.AddDays(-15),
                        HealthyEggs = 0,
                        RottenEggs = 375,
                        HatchedEggs = 1125,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-15)
                        // 0 + 375 + 1125 = 1500
                    },

                    // ===================================================================
                    // 4. EggBatchId = 4 (BP-008: Sanke - Đã nở) → 3 ngày → TotalEggs = 1800
                    //    → FryFish InitialCount = 1314
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 4,
                        DayNumber = DateTime.Now.AddDays(-19),
                        HealthyEggs = 1566,
                        RottenEggs = 234,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-19)
                        // 1566 + 234 + 0 = 1800
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 4,
                        DayNumber = DateTime.Now.AddDays(-18),
                        HealthyEggs = 1380,
                        RottenEggs = 420,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-18)
                        // 1380 + 420 + 0 = 1800
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 4,
                        DayNumber = DateTime.Now.AddDays(-16),
                        HealthyEggs = 0,
                        RottenEggs = 486,
                        HatchedEggs = 1314,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-16)
                        // 0 + 486 + 1314 = 1800
                    },

                    // ===================================================================
                    // 5. EggBatchId = 5 (Showa - Lô 1) → 5 ngày → TotalEggs = 4000
                    //    → FryFish InitialCount = 3520
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 5,
                        DayNumber = DateTime.Now.AddDays(-90),
                        HealthyEggs = 3800,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-90)
                        // 3800 + 200 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 5,
                        DayNumber = DateTime.Now.AddDays(-89),
                        HealthyEggs = 3700,
                        RottenEggs = 300,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-89)
                        // 3700 + 300 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 5,
                        DayNumber = DateTime.Now.AddDays(-88),
                        HealthyEggs = 2600,
                        RottenEggs = 400,
                        HatchedEggs = 1000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-88)
                        // 2600 + 400 + 1000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 5,
                        DayNumber = DateTime.Now.AddDays(-86),
                        HealthyEggs = 500,
                        RottenEggs = 500,
                        HatchedEggs = 3000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-86)
                        // 500 + 500 + 3000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 5,
                        DayNumber = DateTime.Now.AddDays(-84),
                        HealthyEggs = 0,
                        RottenEggs = 480,
                        HatchedEggs = 3520,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-84)
                        // 0 + 480 + 3520 = 4000
                    },

                    // ===================================================================
                    // 6. EggBatchId = 6 (Showa - Lô 2) → 5 ngày → TotalEggs = 4000
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 6,
                        DayNumber = DateTime.Now.AddDays(-80),
                        HealthyEggs = 3800,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-80)
                        // 3800 + 200 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 6,
                        DayNumber = DateTime.Now.AddDays(-79),
                        HealthyEggs = 3650,
                        RottenEggs = 350,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-79)
                        // 3650 + 350 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 6,
                        DayNumber = DateTime.Now.AddDays(-78),
                        HealthyEggs = 2400,
                        RottenEggs = 600,
                        HatchedEggs = 1000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-78)
                        // 2400 + 600 + 1000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 6,
                        DayNumber = DateTime.Now.AddDays(-76),
                        HealthyEggs = 400,
                        RottenEggs = 600,
                        HatchedEggs = 3000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-76)
                        // 400 + 600 + 3000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 6,
                        DayNumber = DateTime.Now.AddDays(-74),
                        HealthyEggs = 0,
                        RottenEggs = 480,
                        HatchedEggs = 3520,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-74)
                        // 0 + 480 + 3520 = 4000
                    },

                    // ===================================================================
                    // 7. EggBatchId = 7 (Showa - Lô 3) → 5 ngày → TotalEggs = 4000
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 7,
                        DayNumber = DateTime.Now.AddDays(-70),
                        HealthyEggs = 3800,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-70)
                        // 3800 + 200 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 7,
                        DayNumber = DateTime.Now.AddDays(-69),
                        HealthyEggs = 3700,
                        RottenEggs = 300,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-69)
                        // 3700 + 300 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 7,
                        DayNumber = DateTime.Now.AddDays(-68),
                        HealthyEggs = 2500,
                        RottenEggs = 500,
                        HatchedEggs = 1000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-68)
                        // 2500 + 500 + 1000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 7,
                        DayNumber = DateTime.Now.AddDays(-66),
                        HealthyEggs = 600,
                        RottenEggs = 400,
                        HatchedEggs = 3000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-66)
                        // 600 + 400 + 3000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 7,
                        DayNumber = DateTime.Now.AddDays(-64),
                        HealthyEggs = 0,
                        RottenEggs = 480,
                        HatchedEggs = 3520,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-64)
                        // 0 + 480 + 3520 = 4000
                    },

                    // ===================================================================
                    // 8. EggBatchId = 8 (Showa - Lô 4) → 5 ngày → TotalEggs = 4000
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 8,
                        DayNumber = DateTime.Now.AddDays(-60),
                        HealthyEggs = 3800,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-60)
                        // 3800 + 200 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 8,
                        DayNumber = DateTime.Now.AddDays(-59),
                        HealthyEggs = 3680,
                        RottenEggs = 320,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-59)
                        // 3680 + 320 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 8,
                        DayNumber = DateTime.Now.AddDays(-58),
                        HealthyEggs = 2550,
                        RottenEggs = 450,
                        HatchedEggs = 1000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-58)
                        // 2550 + 450 + 1000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 8,
                        DayNumber = DateTime.Now.AddDays(-56),
                        HealthyEggs = 450,
                        RottenEggs = 550,
                        HatchedEggs = 3000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-56)
                        // 450 + 550 + 3000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 8,
                        DayNumber = DateTime.Now.AddDays(-54),
                        HealthyEggs = 0,
                        RottenEggs = 480,
                        HatchedEggs = 3520,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-54)
                        // 0 + 480 + 3520 = 4000
                    },

                    // ===================================================================
                    // 9. EggBatchId = 9 (Showa - Lô 5) → 5 ngày → TotalEggs = 4000
                    // ===================================================================
                    new IncubationDailyRecord
                    {
                        EggBatchId = 9,
                        DayNumber = DateTime.Now.AddDays(-50),
                        HealthyEggs = 3800,
                        RottenEggs = 200,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-50)
                        // 3800 + 200 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 9,
                        DayNumber = DateTime.Now.AddDays(-49),
                        HealthyEggs = 3750,
                        RottenEggs = 250,
                        HatchedEggs = 0,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-49)
                        // 3750 + 250 + 0 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 9,
                        DayNumber = DateTime.Now.AddDays(-48),
                        HealthyEggs = 2600,
                        RottenEggs = 400,
                        HatchedEggs = 1000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-48)
                        // 2600 + 400 + 1000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 9,
                        DayNumber = DateTime.Now.AddDays(-46),
                        HealthyEggs = 550,
                        RottenEggs = 450,
                        HatchedEggs = 3000,
                        Success = false,
                        UpdatedAt = DateTime.Now.AddDays(-46)
                        // 550 + 450 + 3000 = 4000
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 9,
                        DayNumber = DateTime.Now.AddDays(-44),
                        HealthyEggs = 0,
                        RottenEggs = 480,
                        HatchedEggs = 3520,
                        Success = true,
                        UpdatedAt = DateTime.Now.AddDays(-44)
                        // 0 + 480 + 3520 = 4000
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding FryFishes
            if (!context.FryFishes.Any())
            {
                await context.FryFishes.AddRangeAsync(
                    // BP-007: Kohaku - FryFish đang nuôi
                    new FryFish
                    {
                        BreedingProcessId = 7,
                        InitialCount = 1012,   // từ EggBatch (1500 × 0.90 × 0.75)
                        Status = FryFishStatus.Growing,
                        CurrentSurvivalRate = 0.75,
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = null
                    },

                    // BP-008: Sanke - FryFish đang nuôi
                    new FryFish
                    {
                        BreedingProcessId = 8,
                        InitialCount = 1143,   // 1800 × 0.87 × 0.73
                        Status = FryFishStatus.Growing,
                        CurrentSurvivalRate = 0.73,
                        StartDate = DateTime.Now.AddDays(-16),
                        EndDate = null
                    },

                    // BP-009: Kohaku - Completed
                    new FryFish
                    {
                        BreedingProcessId = 9,
                        InitialCount = 787,     // 1500 × 0.90 × 0.75 × 0.70
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.70,
                        StartDate = DateTime.Now.AddDays(-25),
                        EndDate = DateTime.Now.AddDays(-20)
                    },

                    // BP-010: Sanke - Completed
                    new FryFish
                    {
                        BreedingProcessId = 10,
                        InitialCount = 778,     // 1800 × 0.87 × 0.73 × 0.68
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.68,
                        StartDate = DateTime.Now.AddDays(-26),
                        EndDate = DateTime.Now.AddDays(-21)
                    },

                    // BP-011 → BP-015: Showa (Complete)
                    new FryFish
                    {
                        BreedingProcessId = 11,
                        InitialCount = 2943,    // 3344 × 0.88
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.88,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40)
                    },
                    new FryFish
                    {
                        BreedingProcessId = 12,
                        InitialCount = 2943,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.88,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40)
                    },
                    new FryFish
                    {
                        BreedingProcessId = 13,
                        InitialCount = 2943,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.88,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40)
                    },
                    new FryFish
                    {
                        BreedingProcessId = 14,
                        InitialCount = 2943,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.88,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40)
                    },
                    new FryFish
                    {
                        BreedingProcessId = 15,
                        InitialCount = 2943,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.88,
                        StartDate = DateTime.Now.AddDays(-70),
                        EndDate = DateTime.Now.AddDays(-40)
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding FrySurvivalRecords
            if (!context.FrySurvivalRecords.Any())
            {
                await context.FrySurvivalRecords.AddRangeAsync(

                    // ===================== FryFishId = 1 (InitialCount = 1012) =====================
                    new FrySurvivalRecord { FryFishId = 1, DayNumber = DateTime.Now.AddDays(-15), SurvivalRate = 1.00, CountAlive = 1012, Note = "Vừa nở 100%", Success = false },
                    new FrySurvivalRecord { FryFishId = 1, DayNumber = DateTime.Now.AddDays(-12), SurvivalRate = 0.98, CountAlive = 991, Note = "Giảm nhẹ", Success = false },
                    new FrySurvivalRecord { FryFishId = 1, DayNumber = DateTime.Now.AddDays(-9), SurvivalRate = 0.95, CountAlive = 961, Note = "Ổn định", Success = false },
                    new FrySurvivalRecord { FryFishId = 1, DayNumber = DateTime.Now.AddDays(-6), SurvivalRate = 0.93, CountAlive = 941, Note = "Kiểm tra oxy", Success = false },
                    new FrySurvivalRecord { FryFishId = 1, DayNumber = DateTime.Now.AddDays(-3), SurvivalRate = 0.91, CountAlive = 921, Note = "Chuẩn bị phân loại", Success = true },

                    // ===================== FryFishId = 2 (InitialCount = 1143) =====================
                    new FrySurvivalRecord { FryFishId = 2, DayNumber = DateTime.Now.AddDays(-16), SurvivalRate = 1.00, CountAlive = 1143, Success = false },
                    new FrySurvivalRecord { FryFishId = 2, DayNumber = DateTime.Now.AddDays(-13), SurvivalRate = 0.97, CountAlive = 1108, Success = false },
                    new FrySurvivalRecord { FryFishId = 2, DayNumber = DateTime.Now.AddDays(-10), SurvivalRate = 0.94, CountAlive = 1075, Success = false },
                    new FrySurvivalRecord { FryFishId = 2, DayNumber = DateTime.Now.AddDays(-7), SurvivalRate = 0.92, CountAlive = 1051, Success = false },
                    new FrySurvivalRecord { FryFishId = 2, DayNumber = DateTime.Now.AddDays(-4), SurvivalRate = 0.90, CountAlive = 1028, Success = true },

                    // ===================== FryFishId = 3 (InitialCount = 787) =====================
                    new FrySurvivalRecord { FryFishId = 3, DayNumber = DateTime.Now.AddDays(-20), SurvivalRate = 1.00, CountAlive = 787, Success = false },
                    new FrySurvivalRecord { FryFishId = 3, DayNumber = DateTime.Now.AddDays(-17), SurvivalRate = 0.96, CountAlive = 756, Success = false },
                    new FrySurvivalRecord { FryFishId = 3, DayNumber = DateTime.Now.AddDays(-14), SurvivalRate = 0.94, CountAlive = 739, Success = false },
                    new FrySurvivalRecord { FryFishId = 3, DayNumber = DateTime.Now.AddDays(-11), SurvivalRate = 0.91, CountAlive = 716, Success = false },
                    new FrySurvivalRecord { FryFishId = 3, DayNumber = DateTime.Now.AddDays(-8), SurvivalRate = 0.70, CountAlive = 551, Note = "Hoàn tất 300 con", Success = true },

                    // ===================== FryFishId = 4 (InitialCount = 778) =====================
                    new FrySurvivalRecord { FryFishId = 4, DayNumber = DateTime.Now.AddDays(-21), SurvivalRate = 1.00, CountAlive = 778, Success = false },
                    new FrySurvivalRecord { FryFishId = 4, DayNumber = DateTime.Now.AddDays(-18), SurvivalRate = 0.97, CountAlive = 754, Success = false },
                    new FrySurvivalRecord { FryFishId = 4, DayNumber = DateTime.Now.AddDays(-15), SurvivalRate = 0.94, CountAlive = 731, Success = false },
                    new FrySurvivalRecord { FryFishId = 4, DayNumber = DateTime.Now.AddDays(-12), SurvivalRate = 0.92, CountAlive = 715, Success = false },
                    new FrySurvivalRecord { FryFishId = 4, DayNumber = DateTime.Now.AddDays(-9), SurvivalRate = 0.68, CountAlive = 529, Note = "Hoàn tất 350 con", Success = true },

                    // ===================== FryFishId = 5 → 9 (InitialCount = 2943 mỗi BP) =====================
                    // Lặp cùng pattern
                    // BP-011
                    new FrySurvivalRecord { FryFishId = 5, DayNumber = DateTime.Now.AddDays(-40), SurvivalRate = 1.00, CountAlive = 2943, Success = false },
                    new FrySurvivalRecord { FryFishId = 5, DayNumber = DateTime.Now.AddDays(-35), SurvivalRate = 0.95, CountAlive = 2795, Success = false },
                    new FrySurvivalRecord { FryFishId = 5, DayNumber = DateTime.Now.AddDays(-30), SurvivalRate = 0.92, CountAlive = 2707, Success = false },
                    new FrySurvivalRecord { FryFishId = 5, DayNumber = DateTime.Now.AddDays(-25), SurvivalRate = 0.89, CountAlive = 2619, Success = false },
                    new FrySurvivalRecord { FryFishId = 5, DayNumber = DateTime.Now.AddDays(-20), SurvivalRate = 0.88, CountAlive = 2590, Success = true },

                    // BP-012
                    new FrySurvivalRecord { FryFishId = 6, DayNumber = DateTime.Now.AddDays(-32), SurvivalRate = 1.00, CountAlive = 2943, Success = false },
                    new FrySurvivalRecord { FryFishId = 6, DayNumber = DateTime.Now.AddDays(-28), SurvivalRate = 0.96, CountAlive = 2825, Success = false },
                    new FrySurvivalRecord { FryFishId = 6, DayNumber = DateTime.Now.AddDays(-24), SurvivalRate = 0.93, CountAlive = 2737, Success = false },
                    new FrySurvivalRecord { FryFishId = 6, DayNumber = DateTime.Now.AddDays(-20), SurvivalRate = 0.90, CountAlive = 2648, Success = false },
                    new FrySurvivalRecord { FryFishId = 6, DayNumber = DateTime.Now.AddDays(-16), SurvivalRate = 0.88, CountAlive = 2590, Success = true },

                    // BP-013
                    new FrySurvivalRecord { FryFishId = 7, DayNumber = DateTime.Now.AddDays(-30), SurvivalRate = 1.00, CountAlive = 2943, Success = false },
                    new FrySurvivalRecord { FryFishId = 7, DayNumber = DateTime.Now.AddDays(-26), SurvivalRate = 0.95, CountAlive = 2795, Success = false },
                    new FrySurvivalRecord { FryFishId = 7, DayNumber = DateTime.Now.AddDays(-22), SurvivalRate = 0.92, CountAlive = 2707, Success = false },
                    new FrySurvivalRecord { FryFishId = 7, DayNumber = DateTime.Now.AddDays(-18), SurvivalRate = 0.89, CountAlive = 2619, Success = false },
                    new FrySurvivalRecord { FryFishId = 7, DayNumber = DateTime.Now.AddDays(-14), SurvivalRate = 0.88, CountAlive = 2590, Success = true },

                    // BP-014
                    new FrySurvivalRecord { FryFishId = 8, DayNumber = DateTime.Now.AddDays(-30), SurvivalRate = 1.00, CountAlive = 2943, Success = false },
                    new FrySurvivalRecord { FryFishId = 8, DayNumber = DateTime.Now.AddDays(-26), SurvivalRate = 0.95, CountAlive = 2795, Success = false },
                    new FrySurvivalRecord { FryFishId = 8, DayNumber = DateTime.Now.AddDays(-22), SurvivalRate = 0.92, CountAlive = 2707, Success = false },
                    new FrySurvivalRecord { FryFishId = 8, DayNumber = DateTime.Now.AddDays(-18), SurvivalRate = 0.89, CountAlive = 2619, Success = false },
                    new FrySurvivalRecord { FryFishId = 8, DayNumber = DateTime.Now.AddDays(-14), SurvivalRate = 0.88, CountAlive = 2590, Success = true },

                    // BP-015
                    new FrySurvivalRecord { FryFishId = 9, DayNumber = DateTime.Now.AddDays(-30), SurvivalRate = 1.00, CountAlive = 2943, Success = false },
                    new FrySurvivalRecord { FryFishId = 9, DayNumber = DateTime.Now.AddDays(-26), SurvivalRate = 0.95, CountAlive = 2795, Success = false },
                    new FrySurvivalRecord { FryFishId = 9, DayNumber = DateTime.Now.AddDays(-22), SurvivalRate = 0.92, CountAlive = 2707, Success = false },
                    new FrySurvivalRecord { FryFishId = 9, DayNumber = DateTime.Now.AddDays(-18), SurvivalRate = 0.89, CountAlive = 2619, Success = false },
                    new FrySurvivalRecord { FryFishId = 9, DayNumber = DateTime.Now.AddDays(-14), SurvivalRate = 0.88, CountAlive = 2590, Success = true }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding ClassificationStages
            if (!context.ClassificationStages.Any())
            {
                await context.ClassificationStages.AddRangeAsync(

                    // ============================
                    // BP-009 (Kohaku Completed)
                    // ============================
                    new ClassificationStage
                    {
                        BreedingProcessId = 9,
                        TotalCount = 843,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 300,
                        ShowQualifiedCount = 250,
                        PondQualifiedCount = 200,
                        CullQualifiedCount = 93,
                        Notes = "Hoàn tất phân loại Kohaku F1",
                        StartDate = DateTime.Now.AddDays(-14),
                        EndDate = DateTime.Now.AddDays(-8)
                    },

                    // ============================
                    // BP-010 (Sanke Completed)
                    // ============================
                    new ClassificationStage
                    {
                        BreedingProcessId = 10,
                        TotalCount = 959,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 335,
                        ShowQualifiedCount = 287,
                        PondQualifiedCount = 239,
                        CullQualifiedCount = 98,
                        Notes = "Hoàn tất phân loại Sanke F1",
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = DateTime.Now.AddDays(-9)
                    },

                    // ============================
                    // BP-011 Showa
                    // ============================
                    new ClassificationStage
                    {
                        BreedingProcessId = 11,
                        TotalCount = 3097,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 1084,
                        ShowQualifiedCount = 929,
                        PondQualifiedCount = 774,
                        CullQualifiedCount = 310,
                        Notes = "Showa F1 – chất lượng tốt",
                        StartDate = DateTime.Now.AddDays(-26),
                        EndDate = DateTime.Now.AddDays(-20)
                    },

                    new ClassificationStage
                    {
                        BreedingProcessId = 12,
                        TotalCount = 3097,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 1084,
                        ShowQualifiedCount = 929,
                        PondQualifiedCount = 774,
                        CullQualifiedCount = 310,
                        Notes = "Showa F1 – chất lượng tốt",
                        StartDate = DateTime.Now.AddDays(-22),
                        EndDate = DateTime.Now.AddDays(-16)
                    },

                    new ClassificationStage
                    {
                        BreedingProcessId = 13,
                        TotalCount = 3097,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 1084,
                        ShowQualifiedCount = 929,
                        PondQualifiedCount = 774,
                        CullQualifiedCount = 310,
                        Notes = "Showa F1 – chất lượng tốt",
                        StartDate = DateTime.Now.AddDays(-20),
                        EndDate = DateTime.Now.AddDays(-14)
                    },

                    new ClassificationStage
                    {
                        BreedingProcessId = 14,
                        TotalCount = 3097,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 1084,
                        ShowQualifiedCount = 929,
                        PondQualifiedCount = 774,
                        CullQualifiedCount = 310,
                        Notes = "Showa F1 – màu sắc ổn định",
                        StartDate = DateTime.Now.AddDays(-20),
                        EndDate = DateTime.Now.AddDays(-14)
                    },

                    new ClassificationStage
                    {
                        BreedingProcessId = 15,
                        TotalCount = 3097,
                        Status = ClassificationStatus.Success,
                        HighQualifiedCount = 1084,
                        ShowQualifiedCount = 929,
                        PondQualifiedCount = 774,
                        CullQualifiedCount = 310,
                        Notes = "Showa F1 – đẹp đồng đều",
                        StartDate = DateTime.Now.AddDays(-20),
                        EndDate = DateTime.Now.AddDays(-14)
                    }
                );

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding ClassificationRecord
            if (!context.ClassificationRecords.Any())
            {
                var stages = context.ClassificationStages.ToList();
                var records = new List<ClassificationRecord>();

                foreach (var st in stages)
                {
                    int H = st.HighQualifiedCount ?? 0;
                    int S = st.ShowQualifiedCount ?? 0;
                    int P = st.PondQualifiedCount ?? 0;
                    int C = st.CullQualifiedCount ?? 0;

                    // Chia Cull thành 2 phần C1 + C2
                    int C1 = C / 2;
                    int C2 = C - C1;

                    // ================================
                    // Stage 1 → C1
                    // ================================
                    records.Add(new ClassificationRecord
                    {
                        ClassificationStageId = st.Id,
                        StageNumber = 1,
                        HighQualifiedCount = 0,
                        ShowQualifiedCount = 0,
                        PondQualifiedCount = 0,
                        CullQualifiedCount = C1,
                        Notes = $"Lần 1 - loại lần 1 ({C1})",
                        CreateAt = st.StartDate
                    });

                    // ================================
                    // Stage 2 → C2
                    // ================================
                    records.Add(new ClassificationRecord
                    {
                        ClassificationStageId = st.Id,
                        StageNumber = 2,
                        HighQualifiedCount = 0,
                        ShowQualifiedCount = 0,
                        PondQualifiedCount = 0,
                        CullQualifiedCount = C2,
                        Notes = $"Lần 2 - loại lần 2 ({C2})",
                        CreateAt = st.StartDate.AddDays(1)
                    });

                    // ================================
                    // Stage 3 → POND
                    // ================================
                    records.Add(new ClassificationRecord
                    {
                        ClassificationStageId = st.Id,
                        StageNumber = 3,
                        HighQualifiedCount = 0,
                        ShowQualifiedCount = 0,
                        PondQualifiedCount = P,
                        CullQualifiedCount = 0,
                        Notes = $"Lần 3 - chọn Pond ({P})",
                        CreateAt = st.StartDate.AddDays(2)
                    });

                    // ================================
                    // Stage 4 → HIGH + SHOW (FINAL)
                    // ================================
                    records.Add(new ClassificationRecord
                    {
                        ClassificationStageId = st.Id,
                        StageNumber = 4,
                        HighQualifiedCount = H,
                        ShowQualifiedCount = S,
                        PondQualifiedCount = 0,
                        CullQualifiedCount = 0,
                        Notes = $"Lần 4 - chọn High+Show ({H}+{S})",
                        CreateAt = st.EndDate
                    });
                }

                await context.ClassificationRecords.AddRangeAsync(records);
                await context.SaveChangesAsync();
            }

            #endregion

            #region Seeding PacketFishes
            if (!context.PacketFishes.Any())
            {
                await context.PacketFishes.AddRangeAsync(
                    new PacketFish
                    {
                        Name = "Gói Kohaku Premium",
                        Description = "Bộ sưu tập cá Kohaku chất lượng cao, kích thước từ 21-25cm",
                        FishPerPacket = 10,
                        PricePerPacket = 5000000,
                        MinSize = 40,
                        MaxSize = 50,
                        AgeMonths = 6,
                        Images = new List<string> { "https://res.cloudinary.com/detykxgzs/image/upload/v1761745730/Screenshot_2025-10-29_204746_mrp212.png" },
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PacketFish
                    {
                        Name = "Gói Sanke Show Grade",
                        Description = "Cá Sanke show grade, màu sắc rực rỡ, kích thước từ 26-30cm",
                        FishPerPacket = 10,
                        PricePerPacket = 8000000,
                        MinSize = 26,
                        MaxSize = 30,
                        AgeMonths = 8,
                        Images = new List<string> { "https://res.cloudinary.com/detykxgzs/image/upload/v1761745730/Screenshot_2025-10-29_204746_mrp212.png" },
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PacketFish
                    {
                        Name = "Gói Showa Bán Thương Phẩm",
                        Description = "Cá Showa dạng bán thương phẩm, kích thước từ 31-40cm",
                        FishPerPacket = 10,
                        PricePerPacket = 12000000,
                        MinSize = 30,
                        MaxSize = 40,
                        AgeMonths = 12,
                        Images = new List<string> { "https://res.cloudinary.com/detykxgzs/image/upload/v1761745730/Screenshot_2025-10-29_204746_mrp212.png" },
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PacketFish
                    {
                        Name = "Gói Ogon Tosai",
                        Description = "Cá Ogon còn nhỏ (tosai), kích thước từ 10-20cm",
                        FishPerPacket = 10,
                        PricePerPacket = 3000000,
                        MinSize = 10,
                        MaxSize = 20,
                        AgeMonths = 3,
                        Images = new List<string> { "https://res.cloudinary.com/detykxgzs/image/upload/v1761745730/Screenshot_2025-10-29_204746_mrp212.png" },
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PacketFish
                    {
                        Name = "Gói Asagi Jumbo",
                        Description = "Cá Asagi kích thước lớn, từ 41-45cm",
                        FishPerPacket = 10,
                        PricePerPacket = 10000000,
                        MinSize = 30,
                        MaxSize = 35,
                        AgeMonths = 18,
                        Images = new List<string> { "https://res.cloudinary.com/detykxgzs/image/upload/v1761745730/Screenshot_2025-10-29_204746_mrp212.png" },
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Orders
            if (customer1 != null && customer2 != null)
            {
                if (!context.Orders.Any())
                {
                    await context.Orders.AddRangeAsync(
                        new Order
                        {
                            CustomerId = customer1.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-5),
                            Status = OrderStatus.PendingPayment,
                            Subtotal = 8000000,
                            ShippingFee = 100000,
                            DiscountAmount = 0,
                            TotalAmount = 8100000
                        },
                        new Order
                        {
                            CustomerId = customer1.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-10),
                            Status = OrderStatus.Completed,
                            Subtotal = 7000000,
                            ShippingFee = 150000,
                            DiscountAmount = 500000,
                            TotalAmount = 6650000
                        },
                        new Order
                        {
                            CustomerId = customer2.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            Status = OrderStatus.Shipped,
                            Subtotal = 12000000,
                            ShippingFee = 200000,
                            DiscountAmount = 0,
                            TotalAmount = 12200000
                        },
                        new Order
                        {
                            CustomerId = customer1.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-5),
                            Status = OrderStatus.Confirmed,
                            Subtotal = 5000000,
                            ShippingFee = 100000,
                            DiscountAmount = 0,
                            TotalAmount = 5100000
                        },
                        new Order
                        {
                            CustomerId = customer2.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-1),
                            Status = OrderStatus.PendingPayment,
                            Subtotal = 3000000,
                            ShippingFee = 50000,
                            DiscountAmount = 0,
                            TotalAmount = 3050000
                        },
                        new Order
                        {
                            CustomerId = customer1.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-7),
                            Status = OrderStatus.Cancelled,
                            Subtotal = 8000000,
                            ShippingFee = 100000,
                            DiscountAmount = 0,
                            TotalAmount = 8100000
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding OrderDetails
            if (!context.OrderDetails.Any())
            {
                await context.OrderDetails.AddRangeAsync(
                    new OrderDetail { OrderId = 1, PacketFishId = 1, Quantity = 10, UnitPrice = 5000000, TotalPrice = 5000000 },
                    new OrderDetail { OrderId = 1, PacketFishId = 1, Quantity = 10, UnitPrice = 5000000, TotalPrice = 5000000 },
                    new OrderDetail { OrderId = 1, PacketFishId = 2, Quantity = 5, UnitPrice = 8000000, TotalPrice = 3000000 },
                    new OrderDetail { OrderId = 2, PacketFishId = 4, Quantity = 20, UnitPrice = 3000000, TotalPrice = 6000000 },
                    new OrderDetail { OrderId = 2, PacketFishId = 5, Quantity = 2, UnitPrice = 10000000, TotalPrice = 1000000 },
                    new OrderDetail { OrderId = 3, PacketFishId = 3, Quantity = 5, UnitPrice = 12000000, TotalPrice = 6000000 },
                    new OrderDetail { OrderId = 3, PacketFishId = 5, Quantity = 5, UnitPrice = 10000000, TotalPrice = 6000000 },
                    new OrderDetail { OrderId = 4, PacketFishId = 1, Quantity = 10, UnitPrice = 5000000, TotalPrice = 5000000 },
                    new OrderDetail { OrderId = 5, PacketFishId = 4, Quantity = 10, UnitPrice = 3000000, TotalPrice = 3000000 },
                    new OrderDetail { OrderId = 6, PacketFishId = 2, Quantity = 5, UnitPrice = 8000000, TotalPrice = 8000000 }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Carts        
            if (customer1 != null && customer2 != null && customer3 != null)
            {
                if (!context.Carts.Any())
                {
                    await context.Carts.AddRangeAsync(
                        new Cart
                        {
                            CustomerId = customer1.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            UpdatedAt = DateTime.UtcNow.AddDays(-1)
                        },
                        new Cart
                        {
                            CustomerId = customer2.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-5),
                            UpdatedAt = DateTime.UtcNow.AddHours(-2)
                        },
                        new Cart
                        {
                            CustomerId = customer3.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(-1),
                            UpdatedAt = DateTime.UtcNow.AddHours(-6)
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding CartItems
            if (!context.CartItems.Any())
            {
                await context.CartItems.AddRangeAsync(
                    new CartItem
                    {
                        CartId = 1,
                        KoiFishId = 1,
                        Quantity = 1,
                        AddedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new CartItem
                    {
                        CartId = 1,
                        PacketFishId = 1,
                        Quantity = 3,
                        AddedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new CartItem
                    {
                        CartId = 2,
                        KoiFishId = 2,
                        Quantity = 1,
                        AddedAt = DateTime.UtcNow.AddHours(-5),
                        UpdatedAt = DateTime.UtcNow.AddHours(-5)
                    },
                    new CartItem
                    {
                        CartId = 2,
                        PacketFishId = 2,
                        Quantity = 1,
                        AddedAt = DateTime.UtcNow.AddHours(-5),
                        UpdatedAt = DateTime.UtcNow.AddHours(-5)
                    },
                    new CartItem
                    {
                        CartId = 2,
                        PacketFishId = 4,
                        Quantity = 5,
                        AddedAt = DateTime.UtcNow.AddHours(-5),
                        UpdatedAt = DateTime.UtcNow.AddHours(-5)
                    },
                    new CartItem
                    {
                        CartId = 3,
                        KoiFishId = 4,
                        Quantity = 1,
                        AddedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddHours(-6)
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Promotions
            if (!context.Promotions.Any())
            {
                await context.Promotions.AddRangeAsync(
                    new Promotion
                    {
                        Code = "WELCOME20",
                        Description = "Giảm giá 20% cho khách hàng mới đăng ký.",
                        ValidFrom = DateTime.UtcNow,
                        ValidTo = DateTime.UtcNow.AddMonths(6),
                        DiscountType = DiscountType.Percentage,
                        DiscountValue = 20,
                        MaxDiscountAmount = 100000,
                        UsageLimit = 1000,
                        IsActive = true
                    },
                    new Promotion
                    {
                        Code = "FREESHIP",
                        Description = "Miễn phí vận chuyển cho đơn hàng từ 500.000đ.",
                        ValidFrom = DateTime.UtcNow,
                        ValidTo = DateTime.UtcNow.AddYears(1),
                        DiscountType = DiscountType.FixedAmount,
                        DiscountValue = 30000,
                        MinimumOrderAmount = 500000,
                        IsActive = true
                    },
                    new Promotion
                    {
                        Code = "SALE1010",
                        Description = "Siêu sale ngày 10/10, giảm 50.000đ cho mọi đơn hàng.",
                        ValidFrom = new DateTime(2025, 10, 10),
                        ValidTo = new DateTime(2025, 10, 11).AddTicks(-1),
                        DiscountType = DiscountType.FixedAmount,
                        DiscountValue = 50000,
                        IsActive = true
                    },
                    new Promotion
                    {
                        Code = "OLDPROMO",
                        Description = "Khuyến mãi cũ đã hết hạn.",
                        ValidFrom = DateTime.UtcNow.AddMonths(-2),
                        ValidTo = DateTime.UtcNow.AddMonths(-1),
                        DiscountType = DiscountType.Percentage,
                        DiscountValue = 15,
                        IsActive = false
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion        
            #region Seeding TaskTemplates
            if (!context.TaskTemplates.Any())
            {
                await context.TaskTemplates.AddRangeAsync(
                    new TaskTemplate
                    {
                        TaskName = "Cho cá ăn buổi sáng (6h)",
                        Description = "Cho cá ăn thức ăn viên, kiểm tra lượng thức ăn thừa từ lần trước.",
                        DefaultDuration = 30,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Cho cá ăn buổi chiều (17h)",
                        Description = "Cho cá ăn thức ăn viên, điều chỉnh lượng ăn dựa trên sức khỏe và thời tiết.",
                        DefaultDuration = 30,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm tra chất lượng nước",
                        Description = "Đo pH, nhiệt độ, DO (Oxy hòa tan), Ammonia (NH3), Nitrite (NO2). Ghi chép kết quả.",
                        DefaultDuration = 45,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm tra trực quan sức khỏe cá",
                        Description = "Quan sát hành vi bơi, da, vảy, mắt, vây. Phát hiện các dấu hiệu bất thường (lờ đờ, cọ mình, đốm lạ).",
                        DefaultDuration = 30,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm tra hệ thống bơm và sục khí",
                        Description = "Nghe tiếng động lạ từ máy bơm, kiểm tra luồng nước, kiểm tra đầu sục khí đảm bảo hoạt động.",
                        DefaultDuration = 15,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Vệ sinh lưới lọc/Drum filter",
                        Description = "Vệ sinh, xịt rửa lưới lọc cơ học (hoặc kiểm tra chu trình xả của drum filter).",
                        DefaultDuration = 60,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Vệ sinh Protein Skimmer",
                        Description = "Vệ sinh cốc đựng bọt bẩn của Skimmer và kiểm tra hoạt động.",
                        DefaultDuration = 30,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm tra sức khỏe cá chuyên sâu (Lấy mẫu)",
                        Description = "Bắt ngẫu nhiên 2-3 cá ở mỗi ao để kiểm tra nhớt (mucus scrape test) dưới kính hiển vi tìm ký sinh trùng.",
                        DefaultDuration = 90,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Thay nước ao định kỳ",
                        Description = "Thay 20-30% lượng nước ao, châm nước sạch đã qua xử lý. Kiểm tra lại nồng độ muối nếu cần.",
                        DefaultDuration = 120,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm kê kho thức ăn",
                        Description = "Kiểm tra số lượng thức ăn tồn kho, hạn sử dụng, và lập kế hoạch đặt hàng mới.",
                        DefaultDuration = 30,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Vệ sinh đáy ao và xả cặn lọc",
                        Description = "Hút cặn bẩn (siphon) dưới đáy ao và xả cặn bẩn trong các khoang lắng của hệ thống lọc.",
                        DefaultDuration = 90,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Bảo dưỡng/Vệ sinh đèn UV",
                        Description = "Kiểm tra và vệ sinh ống thạch anh của đèn UV để đảm bảo hiệu suất diệt khuẩn.",
                        DefaultDuration = 45,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Kiểm kê kho thuốc và hóa chất",
                        Description = "Kiểm tra tồn kho, hạn sử dụng của thuốc tím, muối, thuốc mê, và các hóa chất xử lý nước khác.",
                        DefaultDuration = 45,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Vệ sinh vật liệu lọc (Bakki, Jmat)",
                        Description = "Vệ sinh sơ bộ vật liệu lọc (chỉ dùng nước trong hồ) để tránh tắc nghẽn, không rửa quá sạch làm chết vi sinh.",
                        DefaultDuration = 120,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Tiếp nhận cá mới - Cách ly",
                        Description = "Chuẩn bị ao cách ly, tiếp nhận cá mới, thực hiện quy trình tắm muối/kháng sinh, và bắt đầu theo dõi.",
                        DefaultDuration = 120,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Xử lý cá bệnh (Tắm thuốc)",
                        Description = "Pha thuốc (ví dụ: thuốc tím, Praziquantel) vào bồn riêng và tắm cho cá theo liều lượng và thời gian quy định.",
                        DefaultDuration = 90,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Chuẩn bị ao đẻ (Mùa sinh sản)",
                        Description = "Vệ sinh ao, chuẩn bị giá thể đẻ (lưới, bùi nhùi), điều chỉnh nhiệt độ và ánh sáng.",
                        DefaultDuration = 180,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskTemplate
                    {
                        TaskName = "Bảo dưỡng máy bơm tổng",
                        Description = "Ngắt điện, tháo, kiểm tra, vệ sinh, và bảo dưỡng hệ thống máy bơm chính của farm.",
                        DefaultDuration = 240,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding WorkSchedules for Complete Week
            if (!context.WorkSchedules.Any())
            {
                // Get Monday of current week
                var today = DateTime.Today;
                var currentDayOfWeek = (int)today.DayOfWeek;
                var monday = today.AddDays(-(currentDayOfWeek == 0 ? 6 : currentDayOfWeek - 1));

                var workSchedules = new List<WorkSchedule>();

                // MONDAY (6 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 11, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-7) });

                // TUESDAY (6 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 7, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(1)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-6) });

                // WEDNESDAY (6 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 8, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(2)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-5) });

                // THURSDAY (6 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 13, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(3)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-4) });

                // FRIDAY (6 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 10, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(14, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(4)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-3) });

                // SATURDAY (7 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 11, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 12, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(14, 45), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(5)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Completed, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) });

                // SUNDAY (7 tasks)
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 1, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 30), Status = WorkTaskStatus.InProgress, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 5, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 30), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 3, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 45), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 4, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(9, 15), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 9, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(9, 30), EndTime = new TimeOnly(11, 30), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 14, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(15, 0), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });
                workSchedules.Add(new WorkSchedule { TaskTemplateId = 2, ScheduledDate = DateOnly.FromDateTime(monday.AddDays(6)), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 30), Status = WorkTaskStatus.Pending, CreatedBy = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) });

                await context.WorkSchedules.AddRangeAsync(workSchedules);
                await context.SaveChangesAsync();
            }
            #endregion
            #region Seeding WeeklyScheduleTemplates
            WeeklyScheduleTemplate standardTemplate = null;
            if (!context.WeeklyScheduleTemplates.Any())
            {
                standardTemplate = new WeeklyScheduleTemplate
                {
                    Name = "Lịch Vận Hành Tiêu Chuẩn Tuần",
                    Description = "Lịch mẫu bao gồm các công việc vận hành cốt lõi hàng ngày và các công việc bảo trì hàng tuần cho farm.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.WeeklyScheduleTemplates.AddAsync(standardTemplate);
                await context.SaveChangesAsync();
            }
            else
            {
                standardTemplate = await context.WeeklyScheduleTemplates.FirstOrDefaultAsync();
            }
            #endregion
            #region Seeding WeeklyScheduleTemplateItems
            if (standardTemplate != null)
            {
                if (!context.WeeklyScheduleTemplateItems.Any())
                {
                    await context.WeeklyScheduleTemplateItems.AddRangeAsync(
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 11,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("10:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 7,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("10:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Tuesday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 8,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("10:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Wednesday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 13,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("10:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Thursday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 10,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("14:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Friday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 11,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("10:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 12,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("14:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Saturday,
                            StartTime = TimeOnly.Parse("17:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 1,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("06:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 5,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("07:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 3,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("08:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 4,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("09:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 9,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("09:30"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 14,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("13:00"),
                        },
                        new WeeklyScheduleTemplateItem
                        {
                            WeeklyScheduleTemplateId = standardTemplate.Id,
                            TaskTemplateId = 2,
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = TimeOnly.Parse("17:00"),
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding StaffAssignments for Complete Week
            if (!context.StaffAssignments.Any())
            {
                var staffAssignments = new List<StaffAssignment>();

                // Rotate staff assignments: Staff1(5), Staff2(6), Staff3(7)
                // Pattern: Morning feeding & Evening feeding = 2 staff, Other tasks = 1-2 staff based on complexity

                int wsId = 1; // WorkSchedule ID counter

                // MONDAY - 6 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Đã cho ăn sáng đầy đủ", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 1, StaffId = 6, CompletionNotes = "Hỗ trợ cho ăn", CompletedAt = DateTime.UtcNow.AddDays(-7) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Kiểm tra sức khỏe cá, không phát hiện bệnh", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Đo pH 7.2, DO 8.5mg/L - Tốt", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Máy bơm hoạt động bình thường", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Vệ sinh đáy ao hoàn tất", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 11
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 5, StaffId = 5, CompletionNotes = "Hỗ trợ vệ sinh", CompletedAt = DateTime.UtcNow.AddDays(-7) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Cho ăn chiều đầy đủ", CompletedAt = DateTime.UtcNow.AddDays(-7) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 6, StaffId = 7, CompletionNotes = "Hỗ trợ cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-7) });

                // TUESDAY - 6 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Cho ăn sáng đầy đủ", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 7, StaffId = 7, CompletionNotes = "Hỗ trợ cho ăn", CompletedAt = DateTime.UtcNow.AddDays(-6) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Kiểm tra sức khỏe, cá khỏe mạnh", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Chất lượng nước ổn định", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Hệ thống hoạt động tốt", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Vệ sinh Protein Skimmer hoàn tất", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 7
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-6) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 12, StaffId = 5, CompletionNotes = "Hỗ trợ cho ăn", CompletedAt = DateTime.UtcNow.AddDays(-6) });

                // WEDNESDAY - 6 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Cho ăn sáng", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 13, StaffId = 5, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-5) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Kiểm tra sức khỏe OK", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Nước trong sạch", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Thiết bị bình thường", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Lấy mẫu 3 cá, không phát hiện ký sinh trùng", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 8
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 17, StaffId = 7, CompletionNotes = "Hỗ trợ kiểm tra", CompletedAt = DateTime.UtcNow.AddDays(-5) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-5) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 18, StaffId = 6, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-5) });

                // THURSDAY - 6 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Cho ăn sáng", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 19, StaffId = 6, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-4) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Cá khỏe mạnh", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Chất lượng nước tốt", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Hệ thống OK", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Kiểm kê thuốc hoàn tất, cần đặt mua muối", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 13
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-4) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 24, StaffId = 7, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-4) });

                // FRIDAY - 6 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Cho ăn sáng", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 25, StaffId = 7, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-3) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Sức khỏe cá tốt", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Nước sạch", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Máy bơm bình thường", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Kiểm kê thức ăn, tồn kho đủ dùng", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 10
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-3) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 30, StaffId = 5, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-3) });

                // SATURDAY - 7 tasks (Completed)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Cho ăn sáng", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 1
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 31, StaffId = 5, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-2) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Cá khỏe", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 5
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = "Chất lượng nước OK", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 3
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Thiết bị tốt", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 4
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = "Vệ sinh đáy ao xong", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 11
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 35, StaffId = 7, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-2) });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Vệ sinh đèn UV hoàn tất", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 12
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = "Cho ăn chiều", CompletedAt = DateTime.UtcNow.AddDays(-2) }); // Task 2
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 37, StaffId = 6, CompletionNotes = "Hỗ trợ", CompletedAt = DateTime.UtcNow.AddDays(-2) });

                // SUNDAY - 7 tasks (1 InProgress, 6 Pending)
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = null, CompletedAt = null }); // Task 1 - InProgress
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 38, StaffId = 7, CompletionNotes = null, CompletedAt = null });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = null, CompletedAt = null }); // Task 5 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = null, CompletedAt = null }); // Task 3 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = null, CompletedAt = null }); // Task 4 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 5, CompletionNotes = null, CompletedAt = null }); // Task 9 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 42, StaffId = 6, CompletionNotes = null, CompletedAt = null });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 7, CompletionNotes = null, CompletedAt = null }); // Task 14 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 43, StaffId = 5, CompletionNotes = null, CompletedAt = null });
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = wsId++, StaffId = 6, CompletionNotes = null, CompletedAt = null }); // Task 2 - Pending
                staffAssignments.Add(new StaffAssignment { WorkScheduleId = 44, StaffId = 7, CompletionNotes = null, CompletedAt = null });

                await context.StaffAssignments.AddRangeAsync(staffAssignments);
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding PondAssignments for Complete Week
            if (!context.PondAssignments.Any())
            {
                var pondAssignments = new List<PondAssignment>();

                // 5 Ponds: 1=Ao Chính Fuji, 2=Bể Cách Ly, 3=Ao Phát Triển 1, 4=Ao Sinh Sản 101, 5=Ao Thư Giãn
                // Feeding tasks (1,2) = All active ponds (1,3,5), Health check (5) = All ponds, Water quality (3) = All ponds
                // Equipment check (4) = All ponds, Maintenance tasks = Specific ponds

                for (int wsId = 1; wsId <= 44; wsId++) // 44 total WorkSchedules
                {
                    // Determine task type based on position in week (6-7 tasks per day)
                    int dayIndex = (wsId - 1) / 6; // 0=Mon, 1=Tue, ..., 6=Sun
                    int taskIndex = (wsId - 1) % 6; // Position within day

                    // Task patterns per day (simplified based on our seed data)
                    // Task 0,1,2,3 = daily tasks, Task 4+ = special tasks

                    if (taskIndex == 0) // Morning feeding (Task 1)
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                    }
                    else if (taskIndex == 1) // Health inspection (Task 5)
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 2 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 4 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                    }
                    else if (taskIndex == 2) // Water quality check (Task 3)
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 2 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 4 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                    }
                    else if (taskIndex == 3) // Equipment check (Task 4)
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                    }
                    else if (taskIndex == 4) // Special tasks (maintenance/water change)
                    {
                        // Varies by day - maintenance tasks usually on main ponds
                        if (dayIndex == 0 || dayIndex == 5) // Monday/Saturday - Pond bottom cleaning
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        }
                        else if (dayIndex == 1) // Tuesday - Protein skimmer
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                        }
                        else if (dayIndex == 2) // Wednesday - Deep health check
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        }
                        else if (dayIndex == 6) // Sunday - Water change (9)
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                        }
                        // Thursday = inventory (no pond), Friday = inventory (no pond)
                    }
                    else if (taskIndex == 5 && dayIndex < 6) // Evening feeding (Task 2) - Mon-Sat
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                    }
                    else if (dayIndex == 5 && wsId == 36) // Saturday special task 2 - UV maintenance
                    {
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                        pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                    }
                    else if (dayIndex == 6) // Sunday special tasks
                    {
                        if (wsId == 43) // Filter media cleaning
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                        }
                        else if (wsId == 44) // Evening feeding
                        {
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 1 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 3 });
                            pondAssignments.Add(new PondAssignment { WorkScheduleId = wsId, PondId = 5 });
                        }
                    }
                }

                await context.PondAssignments.AddRangeAsync(pondAssignments);
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding IncidentTypes
            if (!context.IncidentTypes.Any())
            {
                await context.IncidentTypes.AddRangeAsync(
                    new IncidentType
                    {
                        Name = "Bệnh nấm trắng",
                        Description = "Các biểu hiện nấm trắng xuất hiện trên thân hoặc mang của cá.",
                        DefaultSeverity = SeverityLevel.High,
                        RequiresQuarantine = true,
                        AffectsBreeding = true
                    },
                    new IncidentType
                    {
                        Name = "Sự cố chất lượng nước",
                        Description = "Thông số nước vượt ngưỡng an toàn cần xử lý.",
                        DefaultSeverity = SeverityLevel.Medium,
                        RequiresQuarantine = false,
                        AffectsBreeding = false
                    },
                    new IncidentType
                    {
                        Name = "Chấn thương trong ao",
                        Description = "Cá bị trầy xước hoặc chấn thương do thiết bị hoặc va chạm.",
                        DefaultSeverity = SeverityLevel.Low,
                        RequiresQuarantine = false,
                        AffectsBreeding = false
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Incidents
            if (!context.Incidents.Any())
            {
                var fungalType = await context.IncidentTypes.FirstOrDefaultAsync(t => t.Name == "Bệnh nấm trắng");
                var waterType = await context.IncidentTypes.FirstOrDefaultAsync(t => t.Name == "Sự cố chất lượng nước");
                var injuryType = await context.IncidentTypes.FirstOrDefaultAsync(t => t.Name == "Chấn thương trong ao");

                var manager = await context.Users.FirstOrDefaultAsync(u => u.Role == Role.Manager);
                var farmStaff = await context.Users.Where(u => u.Role == Role.FarmStaff).OrderBy(u => u.Id).ToListAsync();
                var ponds = await context.Ponds.OrderBy(p => p.Id).ToListAsync();
                var koiList = await context.KoiFishes.OrderBy(k => k.Id).Take(4).ToListAsync();

                if (fungalType != null && waterType != null && manager != null && farmStaff.Any() && ponds.Any() && koiList.Any())
                {
                    var primaryReporter = farmStaff[0];
                    var secondaryReporter = farmStaff.Count > 1 ? farmStaff[1] : farmStaff[0];
                    var isolationPond = ponds.FirstOrDefault(p => p.PondName == "Bể Cách Ly") ?? ponds[0];
                    var growOutPond = ponds.FirstOrDefault(p => p.PondName == "Ao Phát Triển 1") ?? ponds[0];
                    var lastPond = ponds.Count > 1 ? ponds[ponds.Count - 1] : ponds[0];
                    var fallbackKoi = koiList[0];
                    var koiForInjury = koiList.Count > 3 ? koiList[3] : koiList[koiList.Count - 1];

                    var fungalIncident = new Incident
                    {
                        IncidentTypeId = fungalType.Id,
                        IncidentTitle = "Phát hiện nấm trắng trên đàn Showa",
                        Description = "Xuất hiện đốm trắng trên da của hai cá Showa ở bể cách ly. Cần cách ly và điều trị ngay.",
                        Severity = SeverityLevel.High,
                        Status = IncidentStatus.Reported,
                        OccurredAt = DateTime.UtcNow.AddDays(-4),
                        CreatedAt = DateTime.UtcNow.AddDays(-4),
                        ReportedByUserId = primaryReporter.Id,
                        KoiIncidents = new List<KoiIncident>
                        {
                            new KoiIncident
                            {
                                KoiFishId = koiList[0].Id,
                                AffectedStatus = HealthStatus.Warning,
                                SpecificSymptoms = "Xuất hiện đốm trắng quanh mang và thân.",
                                RequiresTreatment = true,
                                IsIsolated = true,
                                AffectedFrom = DateTime.UtcNow.AddDays(-4),
                                TreatmentNotes = "Tắm thuốc xanh methylen và tăng nhiệt độ."
                            },
                            new KoiIncident
                            {
                                KoiFishId = koiList.Count > 1 ? koiList[1].Id : fallbackKoi.Id,
                                AffectedStatus = HealthStatus.Healthy,
                                SpecificSymptoms = "Vây hậu môn bị sưng đỏ, cá kém ăn.",
                                RequiresTreatment = true,
                                IsIsolated = true,
                                AffectedFrom = DateTime.UtcNow.AddDays(-3),
                                TreatmentNotes = "Theo dõi phản ứng thuốc sau 3 giờ."
                            }
                        },
                        PondIncidents = new List<PondIncident>
                        {
                            new PondIncident
                            {
                                PondId = isolationPond.Id,
                                EnvironmentalChanges = "Amonia tăng nhẹ sau khi cho ăn.",
                                RequiresWaterChange = true,
                                FishDiedCount = 0,
                                CorrectiveActions = "Thay 30% nước và bổ sung muối 0.3%.",
                                Notes = "Đã bật hệ thống sưởi để ổn định nhiệt độ."
                            }
                        }
                    };

                    var waterIncident = new Incident
                    {
                        IncidentTypeId = waterType.Id,
                        IncidentTitle = "pH giảm đột ngột tại ao phát triển",
                        Description = "pH đo sáng nay là 6.4, thấp hơn mức an toàn. Nghi ngờ mưa lớn làm loãng đệm.",
                        Severity = SeverityLevel.Medium,
                        Status = IncidentStatus.Resolved,
                        OccurredAt = DateTime.UtcNow.AddDays(-2),
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2),
                        ReportedByUserId = secondaryReporter.Id,
                        ResolvedByUserId = manager.Id,
                        ResolvedAt = DateTime.UtcNow.AddDays(-1),
                        ResolutionNotes = "Đã bón dolomite, tăng sục khí và pH ổn định sau 12 giờ.",
                        KoiIncidents = new List<KoiIncident>
                        {
                            new KoiIncident
                            {
                                KoiFishId = koiList.Count > 2 ? koiList[2].Id : fallbackKoi.Id,
                                AffectedStatus = HealthStatus.Dead,
                                SpecificSymptoms = "Bơi gần mặt nước, thở gấp.",
                                RequiresTreatment = false,
                                IsIsolated = false,
                                AffectedFrom = DateTime.UtcNow.AddDays(-2),
                                TreatmentNotes = "Theo dõi hành vi, chưa cần can thiệp."
                            }
                        },
                        PondIncidents = new List<PondIncident>
                        {
                            new PondIncident
                            {
                                PondId = growOutPond.Id,
                                EnvironmentalChanges = "pH giảm xuống 6.4, ORP thấp.",
                                RequiresWaterChange = false,
                                FishDiedCount = null,
                                CorrectiveActions = "Bổ sung 5kg dolomite và tăng sục khí.",
                                Notes = "Theo dõi pH mỗi 3 giờ trong ngày."
                            }
                        }
                    };

                    var incidentsToSeed = new List<Incident> { fungalIncident, waterIncident };

                    if (injuryType != null)
                    {
                        var injuryIncident = new Incident
                        {
                            IncidentTypeId = injuryType.Id,
                            IncidentTitle = "Cá trầy xước do máng ăn lỏng",
                            Description = "Một cá bị trầy nhẹ phần thân do cạnh máng ăn bị lỏng và bám rêu.",
                            Severity = SeverityLevel.Low,
                            Status = IncidentStatus.Reported,
                            OccurredAt = DateTime.UtcNow.AddDays(-1),
                            CreatedAt = DateTime.UtcNow.AddDays(-1),
                            ReportedByUserId = primaryReporter.Id,
                            KoiIncidents = new List<KoiIncident>
                            {
                                new KoiIncident
                                {
                                    KoiFishId = koiForInjury.Id,
                                    AffectedStatus = HealthStatus.Sick,
                                    SpecificSymptoms = "Trầy nhẹ bên hông phải.",
                                    RequiresTreatment = true,
                                    IsIsolated = false,
                                    AffectedFrom = DateTime.UtcNow.AddDays(-1),
                                    TreatmentNotes = "Bôi povidine và kiểm tra lại sau 24 giờ."
                                }
                            },
                            PondIncidents = new List<PondIncident>
                            {
                                new PondIncident
                                {
                                    PondId = lastPond.Id,
                                    EnvironmentalChanges = "Thiết bị máng ăn bị lỏng, có rêu bám.",
                                    RequiresWaterChange = false,
                                    FishDiedCount = 0,
                                    CorrectiveActions = "Vệ sinh và cố định lại máng ăn.",
                                    Notes = "Yêu cầu kiểm tra lại trong ca tối."
                                }
                            }
                        };

                        incidentsToSeed.Add(injuryIncident);
                    }

                    await context.Incidents.AddRangeAsync(incidentsToSeed);
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding ShippingBoxes
            if (!context.ShippingBoxes.Any())
            {
                await context.ShippingBoxes.AddRangeAsync(
                    new ShippingBox
                    {
                        Name = "Mini Box",
                        WeightCapacityLb = 15,
                        Fee = 2500000m,
                        MaxKoiCount = 5,
                        MaxKoiSizeInch = 6,
                        Notes = "Up to 5 tosai (koi under 1 year old and about 6 in. long)",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingBox
                    {
                        Name = "Medium Box",
                        WeightCapacityLb = 50,
                        Fee = 4800000m,
                        MaxKoiCount = 3,
                        MaxKoiSizeInch = 16,
                        Notes = "Up to three (3) koi 16 in. or less",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingBox
                    {
                        Name = "Large Box",
                        WeightCapacityLb = 55,
                        Fee = 5300000m,
                        MaxKoiCount = 4,
                        MaxKoiSizeInch = 16,
                        Notes = "Up to four (4) koi 16 in. or less",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingBox
                    {
                        Name = "Large Box (Single)",
                        WeightCapacityLb = 70,
                        Fee = 6500000m,
                        MaxKoiCount = 1,
                        MaxKoiSizeInch = 25,
                        Notes = "1 Koi (25.59 inch, 65cm or less)",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingBox
                    {
                        Name = "Extra Large Box",
                        WeightCapacityLb = 70,
                        Fee = 8300000m,
                        MaxKoiCount = null,
                        MaxKoiSizeInch = null,
                        Notes = "At the farm manager's discretion",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding ShippingBoxRules
            if (!context.ShippingBoxRules.Any())
            {
                var miniBox = await context.ShippingBoxes.FirstOrDefaultAsync(b => b.Name == "Mini Box");
                var mediumBox = await context.ShippingBoxes.FirstOrDefaultAsync(b => b.Name == "Medium Box");
                var largeBox = await context.ShippingBoxes.FirstOrDefaultAsync(b => b.Name == "Large Box");
                var largeSingleBox = await context.ShippingBoxes.FirstOrDefaultAsync(b => b.Name == "Large Box (Single)");

                if (miniBox != null && mediumBox != null && largeBox != null && largeSingleBox != null)
                {
                    await context.ShippingBoxRules.AddRangeAsync(
                        new ShippingBoxRule
                        {
                            ShippingBoxId = miniBox.Id,
                            RuleType = ShippingRuleType.ByAge,
                            MaxCount = 5,
                            MaxLengthCm = 15,
                            ExtraInfo = "Tosai only (under 1 year old)",
                            Priority = 1,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new ShippingBoxRule
                        {
                            ShippingBoxId = mediumBox.Id,
                            RuleType = ShippingRuleType.ByCount,
                            MaxCount = 3,
                            MaxLengthCm = 40,
                            ExtraInfo = "Standard koi up to 16 inches",
                            Priority = 1,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new ShippingBoxRule
                        {
                            ShippingBoxId = largeBox.Id,
                            RuleType = ShippingRuleType.ByCount,
                            MaxCount = 4,
                            MaxLengthCm = 40,
                            ExtraInfo = "Standard koi up to 16 inches",
                            Priority = 1,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new ShippingBoxRule
                        {
                            ShippingBoxId = largeSingleBox.Id,
                            RuleType = ShippingRuleType.ByMaxLength,
                            MaxCount = 1,
                            MaxLengthCm = 65,
                            MinLengthCm = 41,
                            ExtraInfo = "Single large koi",
                            Priority = 1,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
            #endregion

            #region Seeding ShippingDistance
            if (!context.ShippingDistances.Any())
            {
                await context.ShippingDistances.AddRangeAsync(
                    new ShippingDistance
                    {
                        Name = "Nội thành",
                        MinDistanceKm = 0,
                        MaxDistanceKm = 10,
                        PricePerKm = 5000m,
                        BaseFee = 30000m,
                        Description = "Giao hàng trong nội thành, bán kính 10km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingDistance
                    {
                        Name = "Ngoại thành gần",
                        MinDistanceKm = 11,
                        MaxDistanceKm = 30,
                        PricePerKm = 7000m,
                        BaseFee = 50000m,
                        Description = "Giao hàng ngoại thành, từ 11-30km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingDistance
                    {
                        Name = "Ngoại thành xa",
                        MinDistanceKm = 31,
                        MaxDistanceKm = 60,
                        PricePerKm = 10000m,
                        BaseFee = 100000m,
                        Description = "Giao hàng ngoại thành xa, từ 31-60km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingDistance
                    {
                        Name = "Tỉnh lân cận",
                        MinDistanceKm = 61,
                        MaxDistanceKm = 150,
                        PricePerKm = 15000m,
                        BaseFee = 200000m,
                        Description = "Giao hàng tỉnh lân cận, từ 61-150km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingDistance
                    {
                        Name = "Liên tỉnh",
                        MinDistanceKm = 151,
                        MaxDistanceKm = 500,
                        PricePerKm = 20000m,
                        BaseFee = 500000m,
                        Description = "Giao hàng liên tỉnh, từ 151-500km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ShippingDistance
                    {
                        Name = "Liên tỉnh xa",
                        MinDistanceKm = 501,
                        MaxDistanceKm = 1500,
                        PricePerKm = 25000m,
                        BaseFee = 1000000m,
                        Description = "Giao hàng liên tỉnh xa, trên 500km",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion
        }

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
