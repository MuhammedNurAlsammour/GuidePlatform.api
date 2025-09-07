using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.Articles.UpdateArticles
{
  public class UpdateArticlesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateArticlesCommandRequest, TransactionResultPack<UpdateArticlesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateArticlesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateArticlesCommandResponse>> Handle(UpdateArticlesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. articles bul ve kontrol et - 1. articles bul ve kontrol et
        var articles = await _context.articles
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (articles == null)
        {
          return ResultFactory.CreateErrorResult<UpdateArticlesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek articles bulunamadı.",
            "articles not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateArticlesCommandRequest.Map(articles, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.articles.Update(articles);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(articles.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(articles.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateArticlesCommandResponse>(
          new UpdateArticlesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "articles başarıyla güncellendi.",
          $"articles Id: { articles.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateArticlesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "articles güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid articlesId, UpdateArticlesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid articlesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
