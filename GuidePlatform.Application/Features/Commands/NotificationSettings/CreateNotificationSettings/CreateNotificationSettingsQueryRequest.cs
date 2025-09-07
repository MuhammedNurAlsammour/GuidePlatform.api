using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.NotificationSettingsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.NotificationSettings.CreateNotificationSettings
{
  public class CreateNotificationSettingsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateNotificationSettingsCommandResponse>>
  {
    // Bildirim ayarları ile ilgili temel bilgiler - Basic notification settings information
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Setting Type is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Setting Type must be greater than 0")]
    public int SettingType { get; set; }

    public bool IsEnabled { get; set; } = true;

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "settings";

    public static O Map(CreateNotificationSettingsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        UserId = request.UserId,
        SettingType = request.SettingType,
        IsEnabled = request.IsEnabled,
        Icon = request.Icon,
      };
    }
  }
}

