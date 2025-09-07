using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.PaymentsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Payments.CreatePayments
{
  public class CreatePaymentsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreatePaymentsCommandResponse>>
  {
    // Ödeme ile ilgili temel bilgiler - Basic payment information
    [Required(ErrorMessage = "Subscription ID is required")]
    public Guid SubscriptionId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be exactly 3 characters")]
    public string Currency { get; set; } = "SYP";

    [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
    public string? PaymentMethod { get; set; }

    [StringLength(255, ErrorMessage = "Transaction ID cannot exceed 255 characters")]
    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string Status { get; set; } = "pending";

    public string? Notes { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "payment";

    public static O Map(CreatePaymentsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        SubscriptionId = request.SubscriptionId,
        Amount = request.Amount,
        Currency = request.Currency,
        PaymentMethod = request.PaymentMethod,
        TransactionId = request.TransactionId,
        PaymentDate = request.PaymentDate,
        Status = request.Status,
        Notes = request.Notes,
        Icon = request.Icon,
      };
    }
  }
}

