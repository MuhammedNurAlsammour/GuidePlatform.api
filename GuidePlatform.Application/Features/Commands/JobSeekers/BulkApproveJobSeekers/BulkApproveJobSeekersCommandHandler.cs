using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Enums;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Operations;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.BulkApproveJobSeekers
{
  public class BulkApproveJobSeekersCommandHandler : IRequestHandler<BulkApproveJobSeekersCommandRequest, TransactionResultPack<BulkApproveJobSeekersCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public BulkApproveJobSeekersCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<BulkApproveJobSeekersCommandResponse>> Handle(BulkApproveJobSeekersCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var response = new BulkApproveJobSeekersCommandResponse
        {
          TotalRequested = request.JobSeekerIds.Count,
          NewStatus = request.Status.ToString(),
          Reason = request.Reason ?? "Toplu işlem"
        };

        var validIds = new List<Guid>();
        var invalidIds = new List<string>();

        // GUID formatını kontrol et
        foreach (var idString in request.JobSeekerIds)
        {
          if (Guid.TryParse(idString, out var guid))
          {
            validIds.Add(guid);
          }
          else
          {
            invalidIds.Add(idString);
          }
        }

        response.InvalidIds = invalidIds;

        if (!validIds.Any())
        {
          return ResultFactory.CreateErrorResult<BulkApproveJobSeekersCommandResponse>(
              string.Join(",", request.JobSeekerIds), null, "Hata / Geçersiz ID'ler",
              "Hiçbir geçerli JobSeeker ID'si bulunamadı",
              "Tüm ID'ler geçersiz format. Lütfen geçerli GUID formatında ID'ler gönderin.");
        }

        // JobSeeker'ları bul
        var jobSeekers = await _context.jobSeekers
            .Where(x => validIds.Contains(x.Id) && !x.RowIsDeleted)
            .ToListAsync(cancellationToken);

        var foundIds = jobSeekers.Select(x => x.Id).ToList();
        var notFoundIds = validIds.Where(x => !foundIds.Contains(x)).Select(x => x.ToString()).ToList();

        response.NotFoundIds = notFoundIds;

        if (!jobSeekers.Any())
        {
          return ResultFactory.CreateErrorResult<BulkApproveJobSeekersCommandResponse>(
              string.Join(",", request.JobSeekerIds), null, "Hata / Bulunamadı",
              "Hiçbir JobSeeker bulunamadı",
              "Belirtilen ID'lere sahip JobSeeker'lar bulunamadı.");
        }

        var currentDate = DateTime.UtcNow;

        // Her JobSeeker'ı güncelle
        foreach (var jobSeeker in jobSeekers)
        {
          jobSeeker.Status = request.Status;
          jobSeeker.RowUpdatedDate = currentDate;

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

          // Response'a ekle
          response.UpdatedJobSeekers.Add(new JobSeekersDTO
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
          });
        }

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        response.SuccessfullyUpdated = response.UpdatedJobSeekers.Count;
        response.Failed = response.TotalRequested - response.SuccessfullyUpdated;

        var message = $"{response.SuccessfullyUpdated} JobSeeker başarıyla güncellendi";
        if (response.Failed > 0)
        {
          message += $", {response.Failed} işlem başarısız";
        }

        return ResultFactory.CreateSuccessResult(response, string.Join(",", request.JobSeekerIds), null,
            "Başarılı", message, "Toplu işlem tamamlandı.");
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<BulkApproveJobSeekersCommandResponse>(
            string.Join(",", request.JobSeekerIds), null, "Hata / Sistem Hatası",
            $"Toplu JobSeeker güncelleme işlemi sırasında hata oluştu: {ex.Message}",
            "Sistem hatası oluştu. Lütfen tekrar deneyin.");
      }
    }
  }
}
