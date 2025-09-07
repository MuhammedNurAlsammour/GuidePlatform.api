

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings
{
  public class NotificationSettingsDTO : BaseResponseDTO
  {
    // Bildirim ayarları ile ilgili temel bilgiler - Basic notification settings information
    public Guid UserId { get; set; }
    public int SettingType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Icon { get; set; } = "settings";

    // İlişkili veriler - Related data
    public string? UserName { get; set; }
    public string? SettingTypeName { get; set; }
  }
}
