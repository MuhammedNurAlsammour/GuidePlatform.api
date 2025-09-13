using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Enums;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Operations;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.UpdateJobSeekerStatus
{
  public class UpdateJobSeekerStatusCommandHandler : IRequestHandler<UpdateJobSeekerStatusCommandRequest, TransactionResultPack<UpdateJobSeekerStatusCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public UpdateJobSeekerStatusCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<UpdateJobSeekerStatusCommandResponse>> Handle(UpdateJobSeekerStatusCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // JobSeeker'ı bul
        var jobSeeker = await _context.jobSeekers
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id) && !x.RowIsDeleted, cancellationToken);

        if (jobSeeker == null)
        {
          return ResultFactory.CreateErrorResult<UpdateJobSeekerStatusCommandResponse>(
              request.Id, null, "Hata / Bulunamadı", "JobSeeker bulunamadı", "Belirtilen ID'ye sahip JobSeeker bulunamadı.");
        }

        // Önceki durumu kaydet
        var previousStatus = jobSeeker.Status.ToString();
        var newStatus = request.Status.ToString();

        // Durumu güncelle
        jobSeeker.Status = request.Status;
        jobSeeker.RowUpdatedDate = DateTime.UtcNow;

        // Eğer Expired ise RowIsActive'i false yap
        if (request.Status == JobSeekerStatus.Expired)
        {
          jobSeeker.RowIsActive = false;
        }
        // Eğer Active ise RowIsActive'i true yap
        else if (request.Status == JobSeekerStatus.Active)
        {
          jobSeeker.RowIsActive = true;
        }

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        // Response oluştur
        var response = new UpdateJobSeekerStatusCommandResponse
        {
          JobSeeker = new JobSeekersDTO
          {
            Id = jobSeeker.Id,
            BusinessId = jobSeeker.BusinessId,
            FullName = jobSeeker.FullName,
            Description = jobSeeker.Description,
            Phone = jobSeeker.Phone,
            Duration = jobSeeker.Duration,
            IsSponsored = jobSeeker.IsSponsored,
            ProvinceId = jobSeeker.ProvinceId,
            RowCreatedDate = jobSeeker.RowCreatedDate,
            RowUpdatedDate = jobSeeker.RowUpdatedDate,
            RowIsActive = jobSeeker.RowIsActive,
            RowIsDeleted = jobSeeker.RowIsDeleted,
            AuthUserId = jobSeeker.AuthUserId,
            AuthCustomerId = jobSeeker.AuthCustomerId,
            CreateUserId = jobSeeker.CreateUserId,
            UpdateUserId = jobSeeker.UpdateUserId,
            Status = jobSeeker.Status
          },
          PreviousStatus = previousStatus,
          NewStatus = newStatus
        };

        return ResultFactory.CreateSuccessResult(response, request.Id, null, "Başarılı", "JobSeeker durumu başarıyla güncellendi", "İşlem başarıyla tamamlandı.");
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<UpdateJobSeekerStatusCommandResponse>(
            request.Id, null, "Hata / Sistem Hatası", $"JobSeeker durumu güncellenirken hata oluştu: {ex.Message}", "Sistem hatası oluştu. Lütfen tekrar deneyin.");
      }
    }
  }
}
