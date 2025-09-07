using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.NotificationsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Notifications.CreateNotifications
{
  public class CreateNotificationsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateNotificationsCommandResponse>>
  {
    // Bildirim ile ilgili temel bilgiler - Basic notification information
    [Required(ErrorMessage = "Recipient User ID is required")]
    public Guid RecipientUserId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    public string Message { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters")]
    public string NotificationType { get; set; } = "info";

    public bool IsRead { get; set; } = false;

    public DateTime? ReadDate { get; set; }

    [StringLength(500, ErrorMessage = "Action URL cannot exceed 500 characters")]
    public string? ActionUrl { get; set; }

    public Guid? RelatedEntityId { get; set; }

    [StringLength(50, ErrorMessage = "Related entity type cannot exceed 50 characters")]
    public string? RelatedEntityType { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "notifications";

    public static O Map(CreateNotificationsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        RecipientUserId = request.RecipientUserId,
        Title = request.Title,
        Message = request.Message,
        NotificationType = request.NotificationType,
        IsRead = request.IsRead,
        ReadDate = request.ReadDate,
        ActionUrl = request.ActionUrl,
        RelatedEntityId = request.RelatedEntityId,
        RelatedEntityType = request.RelatedEntityType,
        Icon = request.Icon,
      };
    }
  }
}

