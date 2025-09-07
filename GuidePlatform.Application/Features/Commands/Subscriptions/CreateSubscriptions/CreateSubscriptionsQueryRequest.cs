using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.SubscriptionsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Subscriptions.CreateSubscriptions
{
  public class CreateSubscriptionsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateSubscriptionsCommandResponse>>
  {
    // İş kimliği - business_id
    [Required(ErrorMessage = "Business ID is required")]
    public Guid BusinessId { get; set; }

    // Başlangıç tarihi - start_date
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    // Bitiş tarihi - end_date
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    // Tutar - amount
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    // Ödeme durumu - payment_status
    public int PaymentStatus { get; set; } = 0;

    // İkon - icon
    public string? Icon { get; set; } = "subscriptions";

    // Para birimi - currency
    public int Currency { get; set; } = 1;

    // Durum - status
    public int Status { get; set; } = 1;

    // Abonelik türü - subscription_type
    [Required(ErrorMessage = "Subscription type is required")]
    public int SubscriptionType { get; set; }

    public static O Map(CreateSubscriptionsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId,
        BusinessId = request.BusinessId,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        Amount = request.Amount,
        PaymentStatus = request.PaymentStatus,
        Icon = request.Icon,
        Currency = request.Currency,
        Status = request.Status,
        SubscriptionType = request.SubscriptionType,
      };
    }
  }
}

