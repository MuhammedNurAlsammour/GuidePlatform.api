using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;
using Microsoft.Extensions.Logging;

namespace GuidePlatform.Application.Features.Commands.categories.UpdateCategories
{
  public class UpdateCategoriesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateCategoriesCommandRequest, TransactionResultPack<UpdateCategoriesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateCategoriesCommandHandler(
      IApplicationDbContext context, 
      ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateCategoriesCommandResponse>> Handle(UpdateCategoriesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Debug: Kullanıcı bilgilerini kontrol et - Debug: Kullanıcı bilgilerini kontrol et
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. categories bul ve kontrol et - 1. categories bul ve kontrol et
        var categories = await _context.categories
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (categories == null)
        {
          return ResultFactory.CreateErrorResult<UpdateCategoriesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek categories bulunamadı.",
            "categories not found."
          );
        }

        // 🎯 2. Eski durumu kaydet (gerekirse) - 2. Eski durumu kaydet (gerekirse)
        var oldName = categories.Name;
        var oldDescription = categories.Description;

        // 🎯 3. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateCategoriesCommandRequest.Map(categories, request, _currentUserService);

        // 🎯 4. Entity'yi context'e ekle - 4. Entity'yi context'e ekle
        _context.categories.Update(categories);

        // 🎯 5. Değişiklikleri kaydet - 5. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateCategoriesCommandResponse>(
          new UpdateCategoriesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "categories başarıyla güncellendi.",
          $"categories Id: { categories.Id} başarıyla güncellendi."
        );
      }
      catch (DbUpdateException dbEx)
      {
        return ResultFactory.CreateErrorResult<UpdateCategoriesCommandResponse>(
          request.Id,
          null,
          "Veritabanı Hatası",
          "Veritabanı güncelleme işlemi başarısız oldu.",
          dbEx.InnerException?.Message ?? dbEx.Message
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateCategoriesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "categories güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid categoriesId, UpdateCategoriesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid categoriesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
