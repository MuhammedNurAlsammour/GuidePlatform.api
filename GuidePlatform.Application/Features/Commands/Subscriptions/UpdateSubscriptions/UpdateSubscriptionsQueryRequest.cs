using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.SubscriptionsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Subscriptions.UpdateSubscriptions
{
  public class UpdateSubscriptionsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateSubscriptionsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İş kimliği - business_id
    public Guid? BusinessId { get; set; }

    // Başlangıç tarihi - start_date
    public DateTime? StartDate { get; set; }

    // Bitiş tarihi - end_date
    public DateTime? EndDate { get; set; }

    // Tutar - amount
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal? Amount { get; set; }

    // Ödeme durumu - payment_status
    public int? PaymentStatus { get; set; }

    // İkon - icon
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    // Para birimi - currency
    public int? Currency { get; set; }

    // Durum - status
    public int? Status { get; set; }

    // Abonelik türü - subscription_type
    public int? SubscriptionType { get; set; }

    public static O Map(O entity, UpdateSubscriptionsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Yeni özellikler - New properties
      if (request.BusinessId.HasValue)
        entity.BusinessId = request.BusinessId.Value;

      if (request.StartDate.HasValue)
        entity.StartDate = request.StartDate.Value;

      if (request.EndDate.HasValue)
        entity.EndDate = request.EndDate.Value;

      if (request.Amount.HasValue)
        entity.Amount = request.Amount.Value;

      if (request.PaymentStatus.HasValue)
        entity.PaymentStatus = request.PaymentStatus.Value;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (request.Currency.HasValue)
        entity.Currency = request.Currency.Value;

      if (request.Status.HasValue)
        entity.Status = request.Status.Value;

      if (request.SubscriptionType.HasValue)
        entity.SubscriptionType = request.SubscriptionType.Value;

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
