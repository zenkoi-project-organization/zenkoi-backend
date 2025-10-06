
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Zenkoi.API.ConfigExtensions;
using Zenkoi.API.Middlewares;
using Zenkoi.BLL.Helpers.Config;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.EF;
using Zenkoi.DAL.Entities;

namespace Zenkoi.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddDbContext<ZenKoiContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("ZenKoiDB"));
            });
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

            builder.Services.Configure<VnPayConfiguration>(builder.Configuration.GetSection("VnPayConfiguration"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<VnPayConfiguration>>().Value);
        

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<ZenKoiContext>().AddDefaultTokenProviders();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", build =>
                {
                    build.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.AddRepoBase();

            builder.Services.AddUnitOfWork();

            builder.Services.AddMapper();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddBLLServices();        

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    RoleClaimType = "Role"
                };
            });

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

            builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //builder.Services.AddSingleton<PayOS>(provider =>
            //{
            //    var configuration = provider.GetRequiredService<IConfiguration>();

            //    var clientId = configuration["PayOS:ClientId"];
            //    var apiKey = configuration["PayOS:ApiKey"];
            //    var checksumKey = configuration["PayOS:ChecksumKey"];

            //    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
            //    {
            //        throw new ArgumentException("PayOS configuration is missing. Please check appsettings.json");
            //    }

            //    return new PayOS(clientId, apiKey, checksumKey);
            //});


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    services.SeedData().Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }   
          

            app.UseCors("CorsPolicy");

            //app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            if (app.Environment.IsDevelopment()) app.MapGet("/", () => Results.Redirect("/swagger"));
            else if (app.Environment.IsProduction())
            {
                app.MapGet("/", () => Results.Content(
                    """
							<!DOCTYPE html>
							<html lang="en">
							<head>
								<meta charset="UTF-8">
								<meta name="viewport" content="width=device-width, initial-scale=1.0">
								<title>Zenkoi Server</title>
								<style>
									* {
										box-sizing: border-box;
									}
									body {
										margin: 0;
										padding: 0;
										font-family: 'Segoe UI', sans-serif;
										background: linear-gradient(135deg, #1e3a8a, #2563eb);
										color: #f8fafc;
										height: 100vh;
										display: flex;
										align-items: center;
										justify-content: center;
										flex-direction: column;
										text-align: center;
									}
									h1 {
										font-size: 3rem;
										margin-bottom: 0.5rem;
									}
									p {
										font-size: 1.2rem;
										color: #cbd5e1;
									}
									.card {
										background: rgba(255, 255, 255, 0.05);
										padding: 2rem 3rem;
										border-radius: 1rem;
										box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
										backdrop-filter: blur(10px);
									}
									.badge {
										margin-top: 1rem;
										font-size: 0.85rem;
										color: #60a5fa;
										background: rgba(255,255,255,0.08);
										padding: 0.25rem 0.75rem;
										border-radius: 9999px;
										text-transform: uppercase;
										letter-spacing: 1px;
									}
								</style>
							</head>
							<body>
								<div class="card">
									<h1>Zenkoi Sever</h1>
								</div>
							</body>
							</html>
							""", "text/html"
                ));
            }

            app.MapControllers();

            app.Run();
        }
    }
}
