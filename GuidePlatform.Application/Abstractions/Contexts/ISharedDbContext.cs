using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Dtos.SharedApi;

namespace GuidePlatform.Application.Abstractions.Contexts
{
  // Shared schema (storeplatformh) için DbContext interface'i
  public interface ISharedDbContext
  {
    DbSet<SharedProvince> provinces { get; set; }
    DbSet<SharedCountry> countries { get; set; }
    DbSet<SharedDistrict> districts { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}
