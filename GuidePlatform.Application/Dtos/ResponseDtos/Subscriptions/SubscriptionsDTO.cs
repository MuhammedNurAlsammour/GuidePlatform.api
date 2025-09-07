

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions
{
  public class SubscriptionsDTO : BaseResponseDTO
  {
    // İş kimliği - business_id
    public Guid BusinessId { get; set; }

    // Başlangıç tarihi - start_date
    public DateTime StartDate { get; set; }

    // Bitiş tarihi - end_date
    public DateTime EndDate { get; set; }

    // Tutar - amount
    public decimal Amount { get; set; }

    // Ödeme durumu - payment_status
    public int PaymentStatus { get; set; }

    // İkon - icon
    public string? Icon { get; set; }

    // Para birimi - currency
    public int Currency { get; set; }

    // Durum - status
    public int Status { get; set; }

    // Abonelik türü - subscription_type
    public int SubscriptionType { get; set; }

    // İş adı (navigation property için)
    public string? BusinessName { get; set; }

    // Para birimi adı (enum değeri için)
    public string? CurrencyName { get; set; }

    // Durum adı (enum değeri için)
    public string? StatusName { get; set; }

    // Abonelik türü adı (enum değeri için)
    public string? SubscriptionTypeName { get; set; }

    // Ödeme durumu adı (enum değeri için)
    public string? PaymentStatusName { get; set; }
  }
}
