using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using System.Net;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunities
{
  public class UpdateJobOpportunitiesCommandHandler : BaseCommandHandler, IRequestHandler<UpdateJobOpportunitiesCommandRequest, TransactionResultPack<UpdateJobOpportunitiesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateJobOpportunitiesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateJobOpportunitiesCommandResponse>> Handle(UpdateJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 🎯 1. jobOpportunities bul ve kontrol et - 1. jobOpportunities bul ve kontrol et
        var jobOpportunities = await _context.jobOpportunities
          .Where(x => x.Id == Guid.Parse(request.Id))
          .FirstOrDefaultAsync(cancellationToken);

        if (jobOpportunities == null)
        {
          return ResultFactory.CreateErrorResult<UpdateJobOpportunitiesCommandResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "Güncellenecek jobOpportunities bulunamadı.",
            "jobOpportunities not found."
          );
        }

        // 🎯 2. Güncellemeleri uygula - 3. Güncellemeleri uygula
        UpdateJobOpportunitiesCommandRequest.Map(jobOpportunities, request, _currentUserService);

        // 🎯 3. Entity'yi context'e ekle - 3. Entity'yi context'e ekle
        _context.jobOpportunities.Update(jobOpportunities);

        // 🎯 4. Değişiklikleri kaydet - 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // 🎯 5. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(jobOpportunities.Id, request, cancellationToken);

        // 🎯 6. 🎯 Durum değişikliği kontrolü (gerekirse)
        // if (request.Status != null && oldStatus != request.Status)
        // {
        //   await ProcessStatusChangeAsync(jobOpportunities.Id, oldStatus, request.Status, cancellationToken);
        // }

        await _context.SaveChangesAsync(cancellationToken);

        return ResultFactory.CreateSuccessResult<UpdateJobOpportunitiesCommandResponse>(
          new UpdateJobOpportunitiesCommandResponse(),
          null,
          null,
          "İşlem Başarılı",
          "jobOpportunities başarıyla güncellendi.",
          $"jobOpportunities Id: { jobOpportunities.Id} başarıyla güncellendi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateJobOpportunitiesCommandResponse>(
          request.Id,
          null,
          "Hata / İşlem Başarısız",
          "jobOpportunities güncellenirken bir hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid jobOpportunitiesId, UpdateJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar güncelleme, validasyonlar, vb.
    // }

    /// <summary>
    /// Durum değişikliği işlemleri için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessStatusChangeAsync(Guid jobOpportunitiesId, int oldStatus, int newStatus, CancellationToken cancellationToken)
    // {
    //   // Durum değişikliği işlemleri buraya eklenebilir
    //   // Örnek: Envanter güncelleme, bildirim gönderme, vb.
    // }
  }
}
