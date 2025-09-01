using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Maps;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Persistence.Contexts
{
	public class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<CategoriesViewModel> categories { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("guideplatform");
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new CategoriesMap());

			// EmployeeMap kaldırıldı, sadece AuthUserId kullanılıyor
			// modelBuilder.ApplyConfiguration(new EmployeeMap());
		}
	}




}
