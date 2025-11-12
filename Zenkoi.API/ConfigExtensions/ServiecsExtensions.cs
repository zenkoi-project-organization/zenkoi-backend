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
                await TruncateAllTablesExceptMigrationHistory(context);
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
                var manager = new ApplicationUser { FullName = "manager", Role = Role.Manager, UserName = "manager", NormalizedUserName = "MANAGER", Email = "manager@email.com", NormalizedEmail = "MANAGER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP", ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a", PhoneNumber = "0999999999", LockoutEnabled = true };
                await context.Users.AddAsync(manager);
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

                await context.Users.AddRangeAsync(staff1, staff2, staff3);
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
                        AreaName = "Khu A",
                        TotalAreaSQM = 500.5,
                        Description = "Khu nuôi cá koi cao cấp"
                    },
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
                        AreaName = "Khu Thử Nghiệm X",
                        Description = "Khu vực dành cho các quy trình thử nghiệm."
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
                        Description = "Ao dành cho cá bố mẹ sinh sản",
                        Type = TypeOfPond.Paring,
                        RecommendedQuantity = 8000
                    },
                    new PondType
                    {
                        TypeName = "Ao ương cá bột",
                        Description = "Ao ương cá con sau khi nở",
                        Type = TypeOfPond.FryFish,
                        RecommendedQuantity = 5000
                    },
                    new PondType
                    {
                        TypeName = "Show Pond",
                        Description = "Ao trưng bày, chú trọng tính thẩm mỹ và dễ quan sát cá từ trên cao. Thường có ít thực vật.",
                        Type = TypeOfPond.BroodStock,
                        RecommendedQuantity = 2500
                    },
                    new PondType
                    {
                        TypeName = "Grow-out Pond",
                        Description = "Ao nuôi dưỡng và phát triển cá non (tosai) hoặc cá cần tăng kích thước nhanh chóng. Yêu cầu hệ thống lọc mạnh.",
                        Type = TypeOfPond.BroodStock,
                        RecommendedQuantity = 5000
                    },
                    new PondType
                    {
                        TypeName = "Quarantine Tank",
                        Description = "Bể/ao nhỏ dùng để cách ly cá mới hoặc cá bệnh. Cần khử trùng và kiểm soát nhiệt độ nghiêm ngặt.",
                        Type = TypeOfPond.MarketPond,
                        RecommendedQuantity = 150
                    },
                    new PondType
                    {
                        TypeName = "Natural Pond",
                        Description = "Ao tự nhiên, có nhiều cây thủy sinh và bùn đáy. Dùng cho mục đích thư giãn, ít can thiệp kỹ thuật.",
                        Type = TypeOfPond.EggBatch,
                        RecommendedQuantity = 8000
                    },
                    new PondType
                    {
                        TypeName = "Breeding Pond",
                        Description = "Ao chuyên dùng để sinh sản, thường có đáy bằng và các giá thể đặc biệt để cá đẻ trứng.",
                        Type = TypeOfPond.MarketPond,
                        RecommendedQuantity = 1000
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding Ponds
            if (!context.Ponds.Any())
            {
                await context.Ponds.AddRangeAsync(
                     new Pond
                     {
                         AreaId = 1,
                         PondTypeId = 1,
                         PondName = "Ao Chính Fuji",
                         PondStatus = PondStatus.Active,
                         Location = "Cánh Đông, Khu Vườn Zen",
                         CapacityLiters = 15000,
                         DepthMeters = 1.8,
                         LengthMeters = 6.0,
                         WidthMeters = 4.0,
                         CreatedAt = DateTime.UtcNow
                     },
                    new Pond
                    {
                        AreaId = 2,
                        PondTypeId = 3,
                        PondName = "Bể Cách Ly",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Nhà Lọc Kỹ Thuật",
                        CapacityLiters = 500,
                        DepthMeters = 0.8,
                        LengthMeters = 1.2,
                        WidthMeters = 1.0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = 1,
                        PondTypeId = 2,
                        PondName = "Ao Phát Triển 1",
                        PondStatus = PondStatus.Active,
                        Location = "Cánh Tây, Khu Nuôi Dưỡng",
                        CapacityLiters = 25000,
                        DepthMeters = 2.0,
                        LengthMeters = 8.0,
                        WidthMeters = 5.0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = 3,
                        PondTypeId = 5,
                        PondName = "Ao Sinh Sản 101",
                        PondStatus = PondStatus.Empty,
                        Location = "Khu Chuẩn Bị Cá Bố Mẹ",
                        CapacityLiters = 4000,
                        DepthMeters = 1.0,
                        LengthMeters = 3.5,
                        WidthMeters = 3.0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Pond
                    {
                        AreaId = 2,
                        PondTypeId = 4,
                        PondName = "Ao Thư Giãn",
                        PondStatus = PondStatus.Maintenance,
                        Location = "Góc Hồ Sen Lớn",
                        CapacityLiters = 35000,
                        DepthMeters = 2.2,
                        LengthMeters = 10.0,
                        WidthMeters = 5.0,
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
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 3, 15),
                        Description = "Kohaku chất lượng cao, có Hi rõ nét, triển vọng thi đấu.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.Tancho,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 2,
                        RFID = "KOI-0001",
                        SellingPrice = 50000000m,
                        Size = 20,
                        Type = KoiType.High,
                        VarietyId = 1,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0.0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 8, 1),
                        Description = "Sanke cái đang phát triển, Sumi đẹp và cân đối.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.Bozu,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 3,
                        RFID = "KOI-0002",
                        SellingPrice = 35000000m,
                        Size = 26,
                        Type = KoiType.Show,
                        VarietyId = 2,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.GinRin, 
                        MutationRate = 0.8
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 1, 20),
                        Description = "Cá mới nhập khẩu, đang theo dõi vì vết xước nhỏ ở vây.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.Maruten,
                        HealthStatus = HealthStatus.Warning,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 4,
                        RFID = "KOI-0003",
                        SellingPrice = null,
                        Size = 24,
                        Type = KoiType.High,
                        VarietyId = 3,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.Doitsu, 
                        MutationRate = 0.9
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2021, 6, 10),
                        Description = "Ogon ánh kim rực rỡ, kích thước lớn, cá bố mẹ tiềm năng.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 5,
                        RFID = "KOI-0004",
                        SellingPrice = 85000000m,
                        Size = 27,
                        Type = KoiType.Show,
                        VarietyId = 4,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.Metallic, 
                        MutationRate = 0.9
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 4, 5),
                        Description = "Asagi Tosai (cá non) có màu xanh sáng đẹp, đang nuôi dưỡng.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 1,
                        RFID = "KOI-0005",
                        SellingPrice = 12000000m,
                        Size = 23,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 5, 12),
                        Description = "Showa Sanshoku đẹp, Hi và Sumi cân đối.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 2,
                        RFID = "KOI-0006",
                        SellingPrice = 42000000m,
                        Size = 30.5,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 9, 20),
                        Description = "Kohaku với Hi pattern đẹp, bloodline cao cấp.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.Maruten,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 3,
                        RFID = "KOI-0007",
                        SellingPrice = 75000000m,
                        Size = 35.2,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Niigata, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 11, 8),
                        Description = "Sanke cái với Sumi pattern tuyệt đẹp, tiềm năng làm giống.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 3,
                        RFID = "KOI-0008",
                        SellingPrice = 55000000m,
                        Size = 32.8,
                        Type = KoiType.Show,
                        VarietyId = 2,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.GinRin,
                        MutationRate = 0.85
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 2, 14),
                        Description = "Shiro Utsuri đẹp, Sumi đậm nét, đang phát triển tốt.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 4,
                        RFID = "KOI-0009",
                        SellingPrice = 28000000m,
                        Size = 25.5,
                        Type = KoiType.High,
                        VarietyId = 3,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 7, 3),
                        Description = "Yamabuki Ogon với màu vàng kim rực rỡ.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 5,
                        RFID = "KOI-0010",
                        SellingPrice = 38000000m,
                        Size = 29.0,
                        Type = KoiType.High,
                        VarietyId = 4,
                        Origin = "Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.Metallic,
                        MutationRate = 0.95
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 6, 18),
                        Description = "Asagi non với màu xanh nhạt đẹp mắt.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 1,
                        RFID = "KOI-0011",
                        SellingPrice = 15000000m,
                        Size = 18.5,
                        Type = KoiType.High,
                        VarietyId = 5,
                        Origin = "Vietnam Farm",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2022, 12, 5),
                        Description = "Kohaku Grand Champion bloodline, cá bố giống xuất sắc.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.Inazuma,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 2,
                        RFID = "KOI-0012",
                        SellingPrice = 120000000m,
                        Size = 42.5,
                        Type = KoiType.Show,
                        VarietyId = 1,
                        Origin = "Sakai Fish Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2023, 3, 22),
                        Description = "Sanke cái với pattern cân đối, cá mẹ giống chất lượng cao.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.NotForSale,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 3,
                        RFID = "KOI-0013",
                        SellingPrice = 68000000m,
                        Size = 38.0,
                        Type = KoiType.Show,
                        VarietyId = 2,
                        Origin = "Dainichi Koi Farm, Japan",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = true,
                        MutationType = MutationType.GinRin,
                        MutationRate = 0.9
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 8, 10),
                        Description = "Showa con F1 từ breeding process BP-006, đang phát triển tốt.",
                        Gender = Gender.Male,
                        PatternType = KoiPatternType.None,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 4,
                        RFID = "KOI-0014",
                        SellingPrice = 25000000m,
                        Size = 22.0,
                        Type = KoiType.High,
                        VarietyId = 1,
                        Origin = "Vietnam Farm - BP-006",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    },
                    new KoiFish
                    {
                        BirthDate = new DateTime(2024, 8, 10),
                        Description = "Kohaku con F1 từ breeding process BP-006, Hi pattern đẹp.",
                        Gender = Gender.Female,
                        PatternType = KoiPatternType.Tancho,
                        HealthStatus = HealthStatus.Healthy,
                        SaleStatus = SaleStatus.Available,
                        Images = new List<string>
                        {
                "https://topanh.com/wp-content/uploads/2025/05/hinh-anh-con-ca-1-768x494.jpg"
                        },
                        PondId = 4,
                        RFID = "KOI-0015",
                        SellingPrice = 30000000m,
                        Size = 21.5,
                        Type = KoiType.High,
                        VarietyId = 1,
                        Origin = "Vietnam Farm - BP-006",
                        CreatedAt = DateTime.UtcNow,
                        Videos = new List<string>(),
                        IsMutated = false,
                        MutationType = MutationType.None,
                        MutationRate = 0
                    }
                );

                await context.SaveChangesAsync();
            }

            #endregion

            #region Seeding BreedingProcesses
            if (!context.BreedingProcesses.Any())
            {
                await context.BreedingProcesses.AddRangeAsync(
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 4,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Đang ghép cặp cá đực và cái, theo dõi phản ứng.",
                        Result = BreedingResult.Success,
                        Code = "BP-001"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 4,
                        StartDate = DateTime.Now.AddDays(-5),
                        Status = BreedingStatus.Spawned,
                        Note = "Cặp cá đã đẻ trứng, đang thu gom trứng.",
                        Result = BreedingResult.PartialSuccess,
                        FertilizationRate = 85,
                        TotalEggs = 1500,
                        Code = "BP-002"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 4,
                        StartDate = DateTime.Now.AddDays(-10),
                        Status = BreedingStatus.EggBatch,
                        Note = "Trứng đang được ấp, tỷ lệ thụ tinh ổn định.",
                        Result = BreedingResult.Unknown,
                        FertilizationRate = 90,
                        TotalEggs = 2000,
                        Code = "BP-003"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 3,
                        StartDate = DateTime.Now.AddDays(-20),
                        Status = BreedingStatus.FryFish,
                        Note = "Cá bột đã nở, bắt đầu cho ăn vi sinh.",
                        Result = BreedingResult.Unknown,
                        SurvivalRate = 75.5,
                        FertilizationRate = 88,
                        TotalEggs = 2500,
                        Code = "BP-004"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 3,
                        StartDate = DateTime.Now.AddDays(-30),
                        Status = BreedingStatus.Classification,
                        Note = "Phân loại cá bột theo kích thước và màu sắc.",
                        Result = BreedingResult.PartialSuccess,
                        TotalFishQualified = 500,
                        TotalPackage = 5,
                        SurvivalRate = 68.3,
                        FertilizationRate = 90,
                        TotalEggs = 3000,
                        Code = "BP-005"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 12,
                        FemaleKoiId = 13,
                        PondId = 3,
                        StartDate = DateTime.Now.AddDays(-90),
                        EndDate = DateTime.Now.AddDays(-30),
                        Status = BreedingStatus.Complete,
                        Note = "Hoàn tất quy trình, kết quả xuất sắc từ cặp bố mẹ bloodline cao cấp.",
                        Result = BreedingResult.Success,
                        TotalFishQualified = 1200,
                        TotalPackage = 25,
                        SurvivalRate = 88.5,
                        FertilizationRate = 95,
                        TotalEggs = 4000,
                        Code = "BP-006"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 4,
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = DateTime.Now.AddDays(-10),
                        Status = BreedingStatus.Failed,
                        Note = "Quá trình sinh sản thất bại do trứng bị nấm.",
                        Result = BreedingResult.Failed,
                        TotalFishQualified = 0,
                        TotalPackage = 0,
                        SurvivalRate = 0,
                        FertilizationRate = 0,
                        TotalEggs = 500,
                        Code = "BP-007"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 2,
                        PondId = 4,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Ghép cặp Kohaku đực với Sanke cái để theo dõi phản ứng ban đầu.",
                        Result = BreedingResult.Unknown,
                        Code = "BP-008"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 4,
                        FemaleKoiId = 3,
                        PondId = 3,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Bắt đầu ghép cặp Ogon đực và Showa cái, kiểm tra hành vi giao phối.",
                        Result = BreedingResult.Unknown,
                        Code = "BP-009"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 5,
                        FemaleKoiId = 2,
                        PondId = 1,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Cặp Asagi đực và Sanke cái đang được chuẩn bị nước và môi trường sinh sản.",
                        Result = BreedingResult.Unknown,
                        Code = "BP-010"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 1,
                        FemaleKoiId = 3,
                        PondId = 5,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Kohaku đực ghép với Showa cái trong ao thử nghiệm X.",
                        Result = BreedingResult.Unknown,
                        Code = "BP-011"
                    },
                    new BreedingProcess
                    {
                        MaleKoiId = 4,
                        FemaleKoiId = 3,
                        PondId = 2,
                        StartDate = DateTime.Now,
                        Status = BreedingStatus.Pairing,
                        Note = "Cặp Ogon đực và Showa cái trong giai đoạn kiểm tra sức khỏe trước sinh sản.",
                        Result = BreedingResult.Unknown,
                        Code = "BP-012"
                    }
                );
                await context.SaveChangesAsync();

                // Update BreedingProcessId only for offspring (F1 generation) after BreedingProcess is seeded
                // KOI-0012 and KOI-0013 are parents (referenced via MaleKoiId/FemaleKoiId), not products of BP-006
                var koi14 = await context.KoiFishes.FirstOrDefaultAsync(k => k.RFID == "KOI-0014");
                var koi15 = await context.KoiFishes.FirstOrDefaultAsync(k => k.RFID == "KOI-0015");

                if (koi14 != null) koi14.BreedingProcessId = 6;
                if (koi15 != null) koi15.BreedingProcessId = 6;

                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding EggBatches
            if (!context.EggBatches.Any())
            {
                await context.EggBatches.AddRangeAsync(
                    new EggBatch
                    {
                        BreedingProcessId = 1,
                        Quantity = 5000,
                        FertilizationRate = 0.85,
                        Status = EggBatchStatus.Success,
                        SpawnDate = new DateTime(2025, 2, 16),
                        HatchingTime = new DateTime(2025, 2, 22)
                    },
                    new EggBatch
                    {
                        BreedingProcessId = 6,
                        Quantity = 4000,
                        FertilizationRate = 0.95,
                        Status = EggBatchStatus.Success,
                        SpawnDate = DateTime.Now.AddDays(-85),
                        HatchingTime = DateTime.Now.AddDays(-79)
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding IncubationDailyRecords
            if (!context.IncubationDailyRecords.Any())
            {
                await context.IncubationDailyRecords.AddRangeAsync(
                    new IncubationDailyRecord
                    {
                        EggBatchId = 1,
                        DayNumber = DateTime.Now,
                        HealthyEggs = 4800,
                        RottenEggs = 200,
                        HatchedEggs = 0
                    },
                    new IncubationDailyRecord
                    {
                        EggBatchId = 1,
                        DayNumber = DateTime.Now,
                        HealthyEggs = 4700,
                        RottenEggs = 300,
                        HatchedEggs = 0
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding FryFishes
            if (!context.FryFishes.Any())
            {
                await context.FryFishes.AddRangeAsync(
                    new FryFish
                    {
                        BreedingProcessId = 1,
                        InitialCount = 4500,
                        Status = FryFishStatus.Growing,
                        CurrentSurvivalRate = 0.93
                    },
                    new FryFish
                    {
                        BreedingProcessId = 2,
                        InitialCount = 6200,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.89
                    },
                    new FryFish
                    {
                        BreedingProcessId = 6,
                        InitialCount = 3800,
                        Status = FryFishStatus.Completed,
                        CurrentSurvivalRate = 0.885
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding FrySurvivalRecords
            if (!context.FrySurvivalRecords.Any())
            {
                await context.FrySurvivalRecords.AddRangeAsync(
                    new FrySurvivalRecord
                    {
                        FryFishId = 1,
                        DayNumber = DateTime.Now,
                        SurvivalRate = 0.95,
                        CountAlive = 4275
                    },
                    new FrySurvivalRecord
                    {
                        FryFishId = 1,
                        DayNumber = DateTime.Now,
                        SurvivalRate = 0.93,
                        CountAlive = 4185
                    }
                );
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seeding ClassificationStages
            if (!context.ClassificationStages.Any())
            {
                await context.ClassificationStages.AddRangeAsync(
                    new ClassificationStage
                    {
                        BreedingProcessId = 1,
                        TotalCount = 4200,
                        HighQualifiedCount = 0,
                        ShowQualifiedCount = 0,
                        PondQualifiedCount = 0,
                        Notes = "Phân loại lần đầu — nhóm cá khỏe mạnh, màu sắc rõ nét chiếm khoảng 20%.",
                        StartDate = DateTime.Now.AddDays(-10),
                        EndDate = DateTime.Now,
                        Status = Zenkoi.DAL.Enums.ClassificationStatus.Preparing
                    },
                    new ClassificationStage
                    {
                        BreedingProcessId = 2,
                        TotalCount = 3000,
                        HighQualifiedCount = 700,
                        ShowQualifiedCount = 400,
                        PondQualifiedCount = 1900,
                        Notes = "Phân loại lần hai — chọn lọc kỹ hơn, giữ lại 35% cá có tiềm năng.",
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now,
                        Status = Zenkoi.DAL.Enums.ClassificationStatus.Success
                    },
                    new ClassificationStage
                    {
                        BreedingProcessId = 6,
                        TotalCount = 3360,
                        HighQualifiedCount = 1200,
                        ShowQualifiedCount = 800,
                        PondQualifiedCount = 1360,
                        Notes = "Phân loại hoàn tất từ BP-006 - Bloodline cao cấp, tỷ lệ High quality cao.",
                        StartDate = DateTime.Now.AddDays(-45),
                        EndDate = DateTime.Now.AddDays(-30),
                        Status = Zenkoi.DAL.Enums.ClassificationStatus.Stage4
                    }
                );
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
                        Quantity = 2,
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
                        Status = IncidentStatus.Investigating,
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
