using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.Pages.UpdatePages
{
  public class UpdatePagesCommandHandler : BaseCommandHandler, IRequestHandler<UpdatePagesCommandRequest, TransactionResultPack<UpdatePagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdatePagesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdatePagesCommandResponse>> Handle(UpdatePagesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. pages bul ve kontrol et - 1. pages bul ve kontrol et
        var pages = await _context.pages
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (pages == null)
        {
          return ResultFactory.CreateErrorResult<UpdatePagesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek pages bulunamadı.",
            "pages not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdatePagesCommandRequest.Map(pages, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.pages.Update(pages);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(pages.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(pages.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdatePagesCommandResponse>(
          new UpdatePagesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "pages başarıyla güncellendi.",
          $"pages Id: { pages.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdatePagesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "pages güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid pagesId, UpdatePagesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid pagesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
