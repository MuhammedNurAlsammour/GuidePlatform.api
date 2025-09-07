using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class NotificationsViewModel : BaseEntity
  {
    // Bildirim ile ilgili temel bilgiler - Basic notification information
    public Guid RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = "info";
    public bool IsRead { get; set; } = false;
    public DateTime? ReadDate { get; set; }
    public string? ActionUrl { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public string Icon { get; set; } = "notifications";
  }
}