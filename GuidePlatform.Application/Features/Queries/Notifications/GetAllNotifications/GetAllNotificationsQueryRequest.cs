using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllNotifications
{
  public class GetAllNotificationsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllNotificationsQueryResponse>>
  {
    // 🔍 Alıcı kullanıcı kimliği filtresi - Recipient user ID filter
    public Guid? RecipientUserId { get; set; }

    // 🔍 Başlık filtresi - Title filter (içerik arama)
    public string? Title { get; set; }

    // 🔍 Mesaj filtresi - Message filter (içerik arama)
    public string? Message { get; set; }

    // 🔍 Bildirim türü filtresi - Notification type filter
    public string? NotificationType { get; set; }

    // 🔍 Okunma durumu filtresi - Read status filter
    public bool? IsRead { get; set; }

    // 🔍 Okuma tarihi aralığı filtresi - Read date range filter
    public DateTime? ReadDateFrom { get; set; }
    public DateTime? ReadDateTo { get; set; }

    // 🔍 İşlem URL'si filtresi - Action URL filter
    public string? ActionUrl { get; set; }

    // 🔍 İlişkili varlık kimliği filtresi - Related entity ID filter
    public Guid? RelatedEntityId { get; set; }

    // 🔍 İlişkili varlık türü filtresi - Related entity type filter
    public string? RelatedEntityType { get; set; }

    // 🔍 Oluşturulma tarihi aralığı filtresi - Creation date range filter
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
  }
}
