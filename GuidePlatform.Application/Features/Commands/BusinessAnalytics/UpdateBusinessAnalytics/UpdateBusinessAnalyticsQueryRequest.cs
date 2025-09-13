using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessAnalyticsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.UpdateBusinessAnalytics
{
  public class UpdateBusinessAnalyticsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessAnalyticsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

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

    public static O Map(O entity, UpdateBusinessAnalyticsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İş analitik verilerini güncelle
      entity.BusinessId = Guid.Parse(request.BusinessId);
      entity.Date = request.Date.Date; // Sadece tarih kısmını al
      entity.ViewsCount = request.ViewsCount;
      entity.ContactsCount = request.ContactsCount;
      entity.ReviewsCount = request.ReviewsCount;
      entity.FavoritesCount = request.FavoritesCount;
      entity.Icon = request.Icon;

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
