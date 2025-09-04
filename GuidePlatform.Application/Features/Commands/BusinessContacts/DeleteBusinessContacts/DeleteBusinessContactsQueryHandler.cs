using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.DeleteBusinessContacts
{
  public class DeleteBusinessContactsCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessContactsCommandRequest, TransactionResultPack<DeleteBusinessContactsCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessContactsCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessContactsCommandResponse>> Handle(DeleteBusinessContactsCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessContactss = await _context.businessContacts
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessContactss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessContactsCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessContacts bulunamadı.",
            "businessContacts not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessContactsAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessContacts.Remove(businessContactss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessContactsCommandResponse>(
          new DeleteBusinessContactsCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessContacts silindi ve stok geri iade edildi.",
          $"businessContacts Id: businessContacts.Id başarıyla silindi ve envanter güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessContactsCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessContacts silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessContactsAdditionalOperationsAsync(Guid businessContactsId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessContactsId);
      // await _auditService.LogDeletionAsync(businessContactsId, _currentUserService.UserId);
    }
  }
}