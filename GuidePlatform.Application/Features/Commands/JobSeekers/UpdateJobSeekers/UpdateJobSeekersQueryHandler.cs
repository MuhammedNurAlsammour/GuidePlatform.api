using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.UpdateJobSeekers
{
  public class UpdateJobSeekersCommandHandler : BaseCommandHandler, IRequestHandler<UpdateJobSeekersCommandRequest, TransactionResultPack<UpdateJobSeekersCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateJobSeekersCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateJobSeekersCommandResponse>> Handle(UpdateJobSeekersCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. jobSeekers bul ve kontrol et - 1. jobSeekers bul ve kontrol et
        var jobSeekers = await _context.jobSeekers
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (jobSeekers == null)
        {
          return ResultFactory.CreateErrorResult<UpdateJobSeekersCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek jobSeekers bulunamadı.",
            "jobSeekers not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateJobSeekersCommandRequest.Map(jobSeekers, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.jobSeekers.Update(jobSeekers);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(jobSeekers.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(jobSeekers.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateJobSeekersCommandResponse>(
          new UpdateJobSeekersCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "jobSeekers başarıyla güncellendi.",
          $"jobSeekers Id: { jobSeekers.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateJobSeekersCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "jobSeekers güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid jobSeekersId, UpdateJobSeekersCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid jobSeekersId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
