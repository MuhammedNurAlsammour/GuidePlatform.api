using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Articles.DeleteArticles
{
  public class DeleteArticlesCommandHandler : BaseCommandHandler, IRequestHandler<DeleteArticlesCommandRequest, TransactionResultPack<DeleteArticlesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteArticlesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteArticlesCommandResponse>> Handle(DeleteArticlesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var articless = await _context.articles
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (articless == null)
        {
          return ResultFactory.CreateErrorResult<DeleteArticlesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek articles bulunamadı.",
            "articles not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessarticlesAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.articles.Remove(articless);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteArticlesCommandResponse>(
          new DeleteArticlesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "articles silindi.",
          $"articles Id: articles.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteArticlesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "articles silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessarticlesAdditionalOperationsAsync(Guid articlesId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(articlesId);
      // await _auditService.LogDeletionAsync(articlesId, _currentUserService.UserId);
    }
  }
}