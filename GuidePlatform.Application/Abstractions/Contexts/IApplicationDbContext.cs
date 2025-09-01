using GuidePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace GuidePlatform.Application.Abstractions.Contexts
{
	public interface IApplicationDbContext
	{
		public DbSet<CategoriesViewModel> categories { get; set; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
