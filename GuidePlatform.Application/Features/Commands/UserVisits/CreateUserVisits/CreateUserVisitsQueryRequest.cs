using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.UserVisitsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.UserVisits.CreateUserVisits
{
  public class CreateUserVisitsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateUserVisitsCommandResponse>>
  {

    // İşletme ID'si - hangi işletmeyi ziyaret etti
    [Required(ErrorMessage = "BusinessId is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string BusinessId { get; set; } = string.Empty;

    // Ziyaret tarihi (opsiyonel - varsayılan olarak şu anki zaman)
    public DateTime? VisitDate { get; set; }

    // Ziyaret türü (view, click, visit, vb.)
    [StringLength(50, ErrorMessage = "VisitType cannot exceed 50 characters")]
    public string VisitType { get; set; } = "view";

    // İkon türü (varsayılan: visibility)
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "visibility";


    public static O Map(CreateUserVisitsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        VisitDate = request.VisitDate ?? DateTime.UtcNow, // Varsayılan olarak şu anki zaman
        VisitType = string.IsNullOrWhiteSpace(request.VisitType) ? "view" : request.VisitType.Trim(),
        Icon = string.IsNullOrWhiteSpace(request.Icon) ? "visibility" : request.Icon.Trim(),
      };
    }
  }
}

