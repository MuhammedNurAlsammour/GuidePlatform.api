using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;	
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.DeleteBusinessWorkingHours
{
  public class DeleteBusinessWorkingHoursCommandHandler : BaseCommandHandler, IRequestHandler<DeleteBusinessWorkingHoursCommandRequest, TransactionResultPack<DeleteBusinessWorkingHoursCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteBusinessWorkingHoursCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<DeleteBusinessWorkingHoursCommandResponse>> Handle(DeleteBusinessWorkingHoursCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var businessWorkingHourss = await _context.businessWorkingHours
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (businessWorkingHourss == null)
        {
          return ResultFactory.CreateErrorResult<DeleteBusinessWorkingHoursCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Silinecek businessWorkingHours bulunamadı.",
            "businessWorkingHours not found."
          );
        }

        // 🎯 Cascade Delete işlemleri - İlişkili kayıtları sil
        await ProcessbusinessWorkingHoursAdditionalOperationsAsync(Guid.Parse(request.Id), cancellationToken);

        // Siparişi sil
        _context.businessWorkingHours.Remove(businessWorkingHourss);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<DeleteBusinessWorkingHoursCommandResponse>(
          new DeleteBusinessWorkingHoursCommandResponse
          {
            StatusCode = (int)HttpStatusCode.OK
          },
          null,
          null,
          "İşlem Başarılı",
          "businessWorkingHours silindi.",
          $"businessWorkingHours Id: businessWorkingHours.Id başarıyla silindi."  // 
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<DeleteBusinessWorkingHoursCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "businessWorkingHours silinirken bir hata oluştu.",
          ex.Message
        );
      }
    }


    /// <summary>
    /// Ek işlemler - İhtiyaca göre düzenlenebilir
    /// </summary>
    private async Task ProcessbusinessWorkingHoursAdditionalOperationsAsync(Guid businessWorkingHoursId, CancellationToken cancellationToken)
    {
      // Ek işlemler buraya eklenebilir
      // Örnek: 
      // - Email bildirimleri gönderme
      // - Log kayıtları oluşturma
      // - Cache temizleme
      // - İlişkili sistemlerde güncelleme
      // - Rapor oluşturma
      
      // await _emailService.SendDeletionNotificationAsync(businessWorkingHoursId);
      // await _auditService.LogDeletionAsync(businessWorkingHoursId, _currentUserService.UserId);
    }
  }
}