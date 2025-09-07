using GuidePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Karmed.External.Auth.Library.Entities.Identity;


namespace GuidePlatform.Application.Abstractions.Contexts
{
  public interface IApplicationDbContext
  {
    public DbSet<CategoriesViewModel> categories { get; set; }
    public DbSet<BusinessesViewModel> businesses { get; set; }
    public DbSet<BusinessImagesViewModel> businessImages { get; set; }
    public DbSet<BusinessContactsViewModel> businessContacts { get; set; }
    public DbSet<BusinessServicesViewModel> businessServices { get; set; }
    public DbSet<BusinessWorkingHoursViewModel> businessWorkingHours { get; set; }
    public DbSet<BusinessReviewsViewModel> businessReviews { get; set; }
    public DbSet<UserFavoritesViewModel> userFavorites { get; set; }
    public DbSet<UserVisitsViewModel> userVisits { get; set; }
    public DbSet<SubscriptionsViewModel> subscriptions { get; set; }
    public DbSet<PaymentsViewModel> payments { get; set; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}
