using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.NotificationSettingsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.NotificationSettings.UpdateNotificationSettings
{
  public class UpdateNotificationSettingsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateNotificationSettingsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Bildirim ayarları ile ilgili güncellenebilir alanlar - Updatable notification settings fields
    public Guid? UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Setting Type must be greater than 0")]
    public int? SettingType { get; set; }

    public bool? IsEnabled { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateNotificationSettingsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Bildirim ayarları alanlarını güncelle - Update notification settings fields
      if (request.UserId.HasValue)
        entity.UserId = request.UserId.Value;

      if (request.SettingType.HasValue)
        entity.SettingType = request.SettingType.Value;

      if (request.IsEnabled.HasValue)
        entity.IsEnabled = request.IsEnabled.Value;

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
