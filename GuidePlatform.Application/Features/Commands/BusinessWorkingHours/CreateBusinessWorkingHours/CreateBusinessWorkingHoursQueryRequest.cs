using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessWorkingHoursViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.CreateBusinessWorkingHours
{
  public class CreateBusinessWorkingHoursCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessWorkingHoursCommandResponse>>
  {

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


    public static O Map(CreateBusinessWorkingHoursCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      // Saatleri TimeSpan'a çevir
      TimeSpan? openTime = null;
      TimeSpan? closeTime = null;

      if (!string.IsNullOrWhiteSpace(request.OpenTime) && TimeSpan.TryParse(request.OpenTime, out var parsedOpenTime))
        openTime = parsedOpenTime;

      if (!string.IsNullOrWhiteSpace(request.CloseTime) && TimeSpan.TryParse(request.CloseTime, out var parsedCloseTime))
        closeTime = parsedCloseTime;

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        DayOfWeek = request.DayOfWeek,
        OpenTime = openTime,
        CloseTime = closeTime,
        IsClosed = request.IsClosed,
        Icon = string.IsNullOrWhiteSpace(request.Icon) ? "schedule" : request.Icon.Trim(),
      };
    }
  }
}

