using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class NotificationSettingsViewModel : BaseEntity
  {
    // Bildirim ayarları ile ilgili temel bilgiler - Basic notification settings information
    public Guid UserId { get; set; }
    public int SettingType { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Icon { get; set; } = "settings";
  }
}