using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.UserVisitsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.UserVisits.UpdateUserVisits
{
  public class UpdateUserVisitsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateUserVisitsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İşletme ID'si - hangi işletmeyi ziyaret etti
    [Required(ErrorMessage = "BusinessId is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string BusinessId { get; set; } = string.Empty;

    // Ziyaret tarihi (opsiyonel)
    public DateTime? VisitDate { get; set; }

    // Ziyaret türü (view, click, visit, vb.)
    [StringLength(50, ErrorMessage = "VisitType cannot exceed 50 characters")]
    public string VisitType { get; set; } = "view";

    // İkon türü (varsayılan: visibility)
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "visibility";

    public static O Map(O entity, UpdateUserVisitsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İşletme ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      // Ziyaret tarihi güncelleme
      if (request.VisitDate.HasValue)
        entity.VisitDate = request.VisitDate.Value;

      // Ziyaret türü güncelleme
      if (!string.IsNullOrWhiteSpace(request.VisitType))
        entity.VisitType = request.VisitType.Trim();
      else
        entity.VisitType = "view"; // Varsayılan değer

      // İkon güncelleme
      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();
      else
        entity.Icon = "visibility"; // Varsayılan değer

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
