

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.Payments
{
  public class PaymentsDTO : BaseResponseDTO
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

    // İlişkili veriler - Related data
    public string? SubscriptionName { get; set; }
    public string? SubscriptionDescription { get; set; }
  }
}
