using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.UpdateBusinessContacts
{
  public class UpdateBusinessContactsCommandHandler : BaseCommandHandler, IRequestHandler<UpdateBusinessContactsCommandRequest, TransactionResultPack<UpdateBusinessContactsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateBusinessContactsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateBusinessContactsCommandResponse>> Handle(UpdateBusinessContactsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. businessContacts bul ve kontrol et - 1. businessContacts bul ve kontrol et
        var businessContacts = await _context.businessContacts
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessContacts == null)
        {
          return ResultFactory.CreateErrorResult<UpdateBusinessContactsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek businessContacts bulunamadı.",
            "businessContacts not found."
          );
        }

        // 🎯 3. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateBusinessContactsCommandRequest.Map(businessContacts, request, _currentUserService);

        // 🎯 4. Entity'yi context'e ekle - 4. Entity'yi context'e ekle
        _context.businessContacts.Update(businessContacts);

        // 🎯 5. Değişiklikleri kaydet - 5. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 4. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(businessContacts.Id, request, cancellationToken);

        // 🎯 5. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(businessContacts.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateBusinessContactsCommandResponse>(
          new UpdateBusinessContactsCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "businessContacts başarıyla güncellendi.",
          $"businessContacts Id: { businessContacts.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateBusinessContactsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessContacts güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessContactsId, UpdateBusinessContactsCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid businessContactsId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
