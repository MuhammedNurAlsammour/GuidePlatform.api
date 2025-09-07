using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllSubscriptions
{
  public class GetAllSubscriptionsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllSubscriptionsQueryResponse>>
  {
    // Page ve Size artık BasePaginatedQueryRequest'ten geliyor

    // 🔍 İş kimliği filtresi - Business ID filter
    public Guid? BusinessId { get; set; }

    // 🔍 Başlangıç tarihi filtresi - Start date filter
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

    // 🔍 Bitiş tarihi filtresi - End date filter
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }

    // 🔍 Ödeme durumu filtresi - Payment status filter
    public int? PaymentStatus { get; set; }

    // 🔍 Para birimi filtresi - Currency filter
    public int? Currency { get; set; }

    // 🔍 Durum filtresi - Status filter
    public int? Status { get; set; }

    // 🔍 Abonelik türü filtresi - Subscription type filter
    public int? SubscriptionType { get; set; }

    // 🔍 Tutar aralığı filtresi - Amount range filter
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
  }
}
