using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Maps;
using Karmed.External.Auth.Library.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Persistence.Contexts
{
  public class ApplicationDbContext : DbContext, IApplicationDbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<CategoriesViewModel> categories { get; set; }
    public DbSet<BusinessesViewModel> businesses { get; set; }
    public DbSet<BusinessImagesViewModel> businessImages { get; set; }
    public DbSet<BusinessContactsViewModel> businessContacts { get; set; }
    public DbSet<BusinessServicesViewModel> businessServices { get; set; }
    public DbSet<BusinessWorkingHoursViewModel> businessWorkingHours { get; set; }
    public DbSet<BusinessReviewsViewModel> businessReviews { get; set; }
    public DbSet<UserFavoritesViewModel> userFavorites { get; set; }
    public DbSet<UserVisitsViewModel> userVisits { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema("guideplatform");
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new CategoriesMap());
      modelBuilder.ApplyConfiguration(new BusinessesMap());
      modelBuilder.ApplyConfiguration(new BusinessImagesMap());
      modelBuilder.ApplyConfiguration(new BusinessContactsMap());
      modelBuilder.ApplyConfiguration(new BusinessServicesMap());
      modelBuilder.ApplyConfiguration(new BusinessWorkingHoursMap());
      modelBuilder.ApplyConfiguration(new BusinessReviewsMap());
      modelBuilder.ApplyConfiguration(new UserFavoritesMap());
      modelBuilder.ApplyConfiguration(new UserVisitsMap());
    }
  }




}
