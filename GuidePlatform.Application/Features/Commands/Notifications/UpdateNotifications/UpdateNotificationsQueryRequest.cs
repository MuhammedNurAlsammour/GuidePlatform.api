using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.NotificationsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Notifications.UpdateNotifications
{
  public class UpdateNotificationsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateNotificationsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Bildirim ile ilgili güncellenebilir alanlar - Updatable notification fields
    public Guid? RecipientUserId { get; set; }

    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string? Title { get; set; }

    public string? Message { get; set; }

    [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters")]
    public string? NotificationType { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    [StringLength(500, ErrorMessage = "Action URL cannot exceed 500 characters")]
    public string? ActionUrl { get; set; }

    public Guid? RelatedEntityId { get; set; }

    [StringLength(50, ErrorMessage = "Related entity type cannot exceed 50 characters")]
    public string? RelatedEntityType { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateNotificationsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Bildirim alanlarını güncelle - Update notification fields
      if (request.RecipientUserId.HasValue)
        entity.RecipientUserId = request.RecipientUserId.Value;

      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Message))
        entity.Message = request.Message.Trim();

      if (!string.IsNullOrWhiteSpace(request.NotificationType))
        entity.NotificationType = request.NotificationType.Trim();

      if (request.IsRead.HasValue)
        entity.IsRead = request.IsRead.Value;

      if (request.ReadDate.HasValue)
        entity.ReadDate = request.ReadDate.Value;
      else if (request.ReadDate == null)
        entity.ReadDate = null;

      if (!string.IsNullOrWhiteSpace(request.ActionUrl))
        entity.ActionUrl = request.ActionUrl.Trim();
      else if (request.ActionUrl == null)
        entity.ActionUrl = null;

      if (request.RelatedEntityId.HasValue)
        entity.RelatedEntityId = request.RelatedEntityId.Value;
      else if (request.RelatedEntityId == null)
        entity.RelatedEntityId = null;

      if (!string.IsNullOrWhiteSpace(request.RelatedEntityType))
        entity.RelatedEntityType = request.RelatedEntityType.Trim();
      else if (request.RelatedEntityType == null)
        entity.RelatedEntityType = null;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
