using GuidePlatform.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GuidePlatform.Persistence
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(Path.Join(Directory.GetCurrentDirectory(), "../GuidePlatform.API/"))
			.AddJsonFile("appsettings.json")
			.Build();

			DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new();
			dbContextOptionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));

			return new(dbContextOptionsBuilder.Options);

		}
	}
}
