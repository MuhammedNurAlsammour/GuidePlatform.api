using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllPayments
{
  public class GetAllPaymentsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllPaymentsQueryResponse>>
  {
    // 🔍 Abonelik kimliği filtresi - Subscription ID filter
    public Guid? SubscriptionId { get; set; }

    // 🔍 Tutar aralığı filtresi - Amount range filter
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }

    // 🔍 Para birimi filtresi - Currency filter
    public string? Currency { get; set; }

    // 🔍 Ödeme yöntemi filtresi - Payment method filter
    public string? PaymentMethod { get; set; }

    // 🔍 İşlem kimliği filtresi - Transaction ID filter
    public string? TransactionId { get; set; }

    // 🔍 Ödeme tarihi aralığı filtresi - Payment date range filter
    public DateTime? PaymentDateFrom { get; set; }
    public DateTime? PaymentDateTo { get; set; }

    // 🔍 Durum filtresi - Status filter
    public string? Status { get; set; }

    // 🔍 Notlar filtresi - Notes filter (içerik arama)
    public string? Notes { get; set; }
  }
}
