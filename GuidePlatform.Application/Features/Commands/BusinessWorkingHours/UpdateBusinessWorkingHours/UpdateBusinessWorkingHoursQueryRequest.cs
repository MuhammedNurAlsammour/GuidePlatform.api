using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessWorkingHoursViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.UpdateBusinessWorkingHours
{
  public class UpdateBusinessWorkingHoursCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessWorkingHoursCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // İşletme ID'si - hangi işletmeye ait çalışma saatleri
    [Required(ErrorMessage = "BusinessId is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format for BusinessId")]
    public string BusinessId { get; set; } = string.Empty;

    // Haftanın günü (1=Pazartesi, 2=Salı, ..., 7=Pazar)
    [Required(ErrorMessage = "DayOfWeek is required")]
    [Range(1, 7, ErrorMessage = "DayOfWeek must be between 1 and 7")]
    public int DayOfWeek { get; set; }

    // Açılış saati (HH:mm formatında)
    public string? OpenTime { get; set; }

    // Kapanış saati (HH:mm formatında)
    public string? CloseTime { get; set; }

    // O gün kapalı mı?
    public bool IsClosed { get; set; } = false;

    // İkon türü (varsayılan: schedule)
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "schedule";

    public static O Map(O entity, UpdateBusinessWorkingHoursCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // İşletme ID'si güncelleme
      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      // Haftanın günü güncelleme
      entity.DayOfWeek = request.DayOfWeek;

      // Açılış saati güncelleme
      if (!string.IsNullOrWhiteSpace(request.OpenTime) && TimeSpan.TryParse(request.OpenTime, out var parsedOpenTime))
        entity.OpenTime = parsedOpenTime;
      else if (request.OpenTime == null)
        entity.OpenTime = null;

      // Kapanış saati güncelleme
      if (!string.IsNullOrWhiteSpace(request.CloseTime) && TimeSpan.TryParse(request.CloseTime, out var parsedCloseTime))
        entity.CloseTime = parsedCloseTime;
      else if (request.CloseTime == null)
        entity.CloseTime = null;

      // Kapalı durumu güncelleme
      entity.IsClosed = request.IsClosed;

      // İkon güncelleme
      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();
      else
        entity.Icon = "schedule"; // Varsayılan değer

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
