using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Enums;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Operations;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunityStatus
{
  public class UpdateJobOpportunityStatusCommandHandler : IRequestHandler<UpdateJobOpportunityStatusCommandRequest, TransactionResultPack<UpdateJobOpportunityStatusCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateJobOpportunityStatusCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateJobOpportunityStatusCommandResponse>> Handle(UpdateJobOpportunityStatusCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // JobOpportunity'yi bul
        var jobOpportunity = await _context.jobOpportunities
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id) && !x.RowIsDeleted, cancellationToken);

        if (jobOpportunity == null)
        {
          return ResultFactory.CreateErrorResult<UpdateJobOpportunityStatusCommandResponse>(
              request.Id, null, "Hata / Bulunamadı", "JobOpportunity bulunamadı", "Belirtilen ID'ye sahip JobOpportunity bulunamadı.");
        }

        // Önceki durumu kaydet
        var previousStatus = jobOpportunity.Status.ToString();
        var newStatus = request.Status.ToString();

        // Durumu güncelle
        jobOpportunity.Status = request.Status;
        jobOpportunity.RowUpdatedDate = DateTime.UtcNow;

        // Eğer Expired ise RowIsActive'i false yap
        if (request.Status == JobOpportunityStatus.Expired)
        {
          jobOpportunity.RowIsActive = false;
        }
        // Eğer Active ise RowIsActive'i true yap
        else if (request.Status == JobOpportunityStatus.Active)
        {
          jobOpportunity.RowIsActive = true;
        }

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // Response oluştur
        var response = new UpdateJobOpportunityStatusCommandResponse
        {
          JobOpportunity = new JobOpportunitiesDTO
          {
            Id = jobOpportunity.Id,
            BusinessId = jobOpportunity.BusinessId,
            Title = jobOpportunity.Title,
            Description = jobOpportunity.Description,
            Phone = jobOpportunity.Phone,
            Duration = jobOpportunity.Duration,
            IsSponsored = jobOpportunity.IsSponsored,
            ProvinceId = jobOpportunity.ProvinceId,
            RowCreatedDate = jobOpportunity.RowCreatedDate,
            RowUpdatedDate = jobOpportunity.RowUpdatedDate,
            RowIsActive = jobOpportunity.RowIsActive,
            RowIsDeleted = jobOpportunity.RowIsDeleted,
            AuthUserId = jobOpportunity.AuthUserId,
            AuthCustomerId = jobOpportunity.AuthCustomerId,
            CreateUserId = jobOpportunity.CreateUserId,
            UpdateUserId = jobOpportunity.UpdateUserId,
            Status = jobOpportunity.Status
          },
          PreviousStatus = previousStatus,
          NewStatus = newStatus
        };

        return ResultFactory.CreateSuccessResult(response, request.Id, null, "Başarılı", "JobOpportunity durumu başarıyla güncellendi", "İşlem başarıyla tamamlandı.");
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateJobOpportunityStatusCommandResponse>(
            request.Id, null, "Hata / Sistem Hatası", $"JobOpportunity durumu güncellenirken hata oluştu: {ex.Message}", "Sistem hatası oluştu. Lütfen tekrar deneyin.");
      }
    }
  }
}
