using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessServicesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessServices.UpdateBusinessServices
{
  public class UpdateBusinessServicesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessServicesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İşletme ID'si - hangi işletmeye ait hizmet
    [Required(ErrorMessage = "BusinessId is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string BusinessId { get; set; } = string.Empty;

    // Hizmet adı
    [Required(ErrorMessage = "ServiceName is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "ServiceName must be between 1 and 255 characters")]
    public string ServiceName { get; set; } = string.Empty;

    // Hizmet açıklaması
    public string? ServiceDescription { get; set; }

    // Fiyat
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
    public decimal? Price { get; set; }

    // Para birimi (varsayılan: SYP)
    [StringLength(3, ErrorMessage = "Currency cannot exceed 3 characters")]
    public string Currency { get; set; } = "SYP";

    // Hizmet mevcut mu?
    public bool IsAvailable { get; set; } = true;

    // İkon türü (varsayılan: miscellaneous_services)
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "miscellaneous_services";

    public static O Map(O entity, UpdateBusinessServicesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İşletme ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      // Hizmet adı güncelleme
      if (!string.IsNullOrWhiteSpace(request.ServiceName))
        entity.ServiceName = request.ServiceName.Trim();

      // Hizmet açıklaması güncelleme
      if (!string.IsNullOrWhiteSpace(request.ServiceDescription))
        entity.ServiceDescription = request.ServiceDescription.Trim();
      else if (request.ServiceDescription == null)
        entity.ServiceDescription = null; // Explicit null assignment

      // Fiyat güncelleme
      if (request.Price.HasValue)
        entity.Price = request.Price.Value;

      // Para birimi güncelleme
      if (!string.IsNullOrWhiteSpace(request.Currency))
        entity.Currency = request.Currency.Trim().ToUpper();
      else
        entity.Currency = "SYP"; // Varsayılan değer

      // Hizmet mevcut mu güncelleme
      entity.IsAvailable = request.IsAvailable;

      // İkon güncelleme
      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();
      else
        entity.Icon = "miscellaneous_services"; // Varsayılan değer

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
