using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class PaymentsViewModel : BaseEntity
  {
    // Ödeme ile ilgili temel bilgiler - Basic payment information
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "SYP";
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Status { get; set; } = "pending";
    public string? Notes { get; set; }
    public string Icon { get; set; } = "payment";

    // Navigation properties - İlişkili tablolar için
    public SubscriptionsViewModel? Subscription { get; set; }
  }
}