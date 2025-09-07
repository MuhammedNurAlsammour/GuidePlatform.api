using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.PaymentsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Payments.UpdatePayments
{
  public class UpdatePaymentsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdatePaymentsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Ödeme ile ilgili güncellenebilir alanlar - Updatable payment fields
    public Guid? SubscriptionId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal? Amount { get; set; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be exactly 3 characters")]
    public string? Currency { get; set; }

    [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
    public string? PaymentMethod { get; set; }

    [StringLength(255, ErrorMessage = "Transaction ID cannot exceed 255 characters")]
    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string? Status { get; set; }

    public string? Notes { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdatePaymentsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Ödeme alanlarını güncelle - Update payment fields
      if (request.SubscriptionId.HasValue)
        entity.SubscriptionId = request.SubscriptionId.Value;

      if (request.Amount.HasValue)
        entity.Amount = request.Amount.Value;

      if (!string.IsNullOrWhiteSpace(request.Currency))
        entity.Currency = request.Currency.Trim();

      if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
        entity.PaymentMethod = request.PaymentMethod.Trim();
      else if (request.PaymentMethod == null)
        entity.PaymentMethod = null;

      if (!string.IsNullOrWhiteSpace(request.TransactionId))
        entity.TransactionId = request.TransactionId.Trim();
      else if (request.TransactionId == null)
        entity.TransactionId = null;

      if (request.PaymentDate.HasValue)
        entity.PaymentDate = request.PaymentDate.Value;
      else if (request.PaymentDate == null)
        entity.PaymentDate = null;

      if (!string.IsNullOrWhiteSpace(request.Status))
        entity.Status = request.Status.Trim();

      if (!string.IsNullOrWhiteSpace(request.Notes))
        entity.Notes = request.Notes.Trim();
      else if (request.Notes == null)
        entity.Notes = null;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
