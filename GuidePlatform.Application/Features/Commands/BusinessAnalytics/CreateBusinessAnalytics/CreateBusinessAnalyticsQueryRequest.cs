using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessAnalyticsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.CreateBusinessAnalytics
{
  public class CreateBusinessAnalyticsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessAnalyticsCommandResponse>>
  {
    // İş analitik verileri için gerekli alanlar
    [Required(ErrorMessage = "Business ID is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string BusinessId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Views count must be non-negative")]
    public int ViewsCount { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Contacts count must be non-negative")]
    public int ContactsCount { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Reviews count must be non-negative")]
    public int ReviewsCount { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Favorites count must be non-negative")]
    public int FavoritesCount { get; set; } = 0;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "analytics";

    public static O Map(CreateBusinessAnalyticsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        Date = request.Date.Date, // Sadece tarih kısmını al
        ViewsCount = request.ViewsCount,
        ContactsCount = request.ContactsCount,
        ReviewsCount = request.ReviewsCount,
        FavoritesCount = request.FavoritesCount,
        Icon = request.Icon
      };
    }
  }
}

