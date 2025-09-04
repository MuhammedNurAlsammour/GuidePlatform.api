using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.UpdateBusinessWorkingHours
{
  public class UpdateBusinessWorkingHoursCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessWorkingHoursCommandRequest, TransactionResultPack<UpdateBusinessWorkingHoursCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateBusinessWorkingHoursCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateBusinessWorkingHoursCommandResponse>> Handle(UpdateBusinessWorkingHoursCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. businessWorkingHours bul ve kontrol et - 1. businessWorkingHours bul ve kontrol et
        var businessWorkingHours = await _context.businessWorkingHours
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessWorkingHours == null)
        {
          return ResultFactory.CreateErrorResult<UpdateBusinessWorkingHoursCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek businessWorkingHours bulunamadı.",
            "businessWorkingHours not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateBusinessWorkingHoursCommandRequest.Map(businessWorkingHours, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.businessWorkingHours.Update(businessWorkingHours);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(businessWorkingHours.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(businessWorkingHours.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateBusinessWorkingHoursCommandResponse>(
          new UpdateBusinessWorkingHoursCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "businessWorkingHours başarıyla güncellendi.",
          $"businessWorkingHours Id: { businessWorkingHours.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateBusinessWorkingHoursCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessWorkingHours güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessWorkingHoursId, UpdateBusinessWorkingHoursCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid businessWorkingHoursId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
