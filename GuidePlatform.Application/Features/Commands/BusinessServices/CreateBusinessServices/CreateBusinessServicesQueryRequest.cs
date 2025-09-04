using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessServicesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.BusinessServices.CreateBusinessServices
{
  public class CreateBusinessServicesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessServicesCommandResponse>>
  {

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


    public static O Map(CreateBusinessServicesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        ServiceName = request.ServiceName.Trim(),
        ServiceDescription = string.IsNullOrWhiteSpace(request.ServiceDescription) ? null : request.ServiceDescription.Trim(),
        Price = request.Price,
        Currency = string.IsNullOrWhiteSpace(request.Currency) ? "SYP" : request.Currency.Trim().ToUpper(),
        IsAvailable = request.IsAvailable,
        Icon = string.IsNullOrWhiteSpace(request.Icon) ? "miscellaneous_services" : request.Icon.Trim(),
      };
    }
  }
}

