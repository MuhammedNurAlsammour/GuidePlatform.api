using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class SubscriptionsViewModel : BaseEntity
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
  }
}