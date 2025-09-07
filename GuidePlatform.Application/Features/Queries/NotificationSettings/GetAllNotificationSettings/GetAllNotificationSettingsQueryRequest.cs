using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllNotificationSettings
{
  /// <summary>
  /// NotificationSettings için filtreleme ve sayfalama parametreleri
  /// </summary>
  public class GetAllNotificationSettingsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllNotificationSettingsQueryResponse>>
  {
    /// <summary>
    /// Kullanıcı kimliği filtresi - User ID filter
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Ayar türü filtresi - Setting type filter
    /// 0: Email, 1: Push Notification, 2: SMS, 3: WhatsApp, 4: Telegram, 5: In-App Notification
    /// </summary>
    public int? SettingType { get; set; }

    /// <summary>
    /// Etkinlik durumu filtresi - Enabled status filter
    /// true: Aktif, false: Pasif
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    /// Oluşturulma tarihi başlangıç filtresi - Creation date from filter
    /// </summary>
    public DateTime? CreatedDateFrom { get; set; }

    /// <summary>
    /// Oluşturulma tarihi bitiş filtresi - Creation date to filter
    /// </summary>
    public DateTime? CreatedDateTo { get; set; }

    /// <summary>
    /// Güncellenme tarihi başlangıç filtresi - Update date from filter
    /// </summary>
    public DateTime? UpdatedDateFrom { get; set; }

    /// <summary>
    /// Güncellenme tarihi bitiş filtresi - Update date to filter
    /// </summary>
    public DateTime? UpdatedDateTo { get; set; }
  }
}
