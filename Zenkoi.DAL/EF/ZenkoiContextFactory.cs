using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Zenkoi.DAL.EF
{
	internal class ZenkoiContextFactory : IDesignTimeDbContextFactory<ZenKoiContext>
	{
		public ZenKoiContext CreateDbContext(string[] args)
		{
			var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ZenKoi.API");
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var builder = new DbContextOptionsBuilder<ZenKoiContext>();
			var connectionString = configuration.GetConnectionString("ZenKoiDB");

			builder.UseSqlServer(connectionString);

			return new ZenKoiContext(builder.Options);
		}
	}


}
