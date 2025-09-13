using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Domain.Entities;
using GuidePlatform.Domain.Enums;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Operations;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.BulkApproveJobOpportunities
{
  public class BulkApproveJobOpportunitiesCommandHandler : IRequestHandler<BulkApproveJobOpportunitiesCommandRequest, TransactionResultPack<BulkApproveJobOpportunitiesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;

    public BulkApproveJobOpportunitiesCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<BulkApproveJobOpportunitiesCommandResponse>> Handle(BulkApproveJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        var response = new BulkApproveJobOpportunitiesCommandResponse
        {
          TotalRequested = request.JobOpportunityIds.Count,
          NewStatus = request.Status.ToString(),
          Reason = request.Reason ?? "Toplu işlem"
        };

        var validIds = new List<Guid>();
        var invalidIds = new List<string>();

        // GUID formatını kontrol et
        foreach (var idString in request.JobOpportunityIds)
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
          return ResultFactory.CreateErrorResult<BulkApproveJobOpportunitiesCommandResponse>(
              string.Join(",", request.JobOpportunityIds), null, "Hata / Geçersiz ID'ler",
              "Hiçbir geçerli JobOpportunity ID'si bulunamadı",
              "Tüm ID'ler geçersiz format. Lütfen geçerli GUID formatında ID'ler gönderin.");
        }

        // JobOpportunity'leri bul
        var jobOpportunities = await _context.jobOpportunities
            .Where(x => validIds.Contains(x.Id) && !x.RowIsDeleted)
            .ToListAsync(cancellationToken);

        var foundIds = jobOpportunities.Select(x => x.Id).ToList();
        var notFoundIds = validIds.Where(x => !foundIds.Contains(x)).Select(x => x.ToString()).ToList();

        response.NotFoundIds = notFoundIds;

        if (!jobOpportunities.Any())
        {
          return ResultFactory.CreateErrorResult<BulkApproveJobOpportunitiesCommandResponse>(
              string.Join(",", request.JobOpportunityIds), null, "Hata / Bulunamadı",
              "Hiçbir JobOpportunity bulunamadı",
              "Belirtilen ID'lere sahip JobOpportunity'ler bulunamadı.");
        }

        var currentDate = DateTime.UtcNow;

        // Her JobOpportunity'yi güncelle
        foreach (var jobOpportunity in jobOpportunities)
        {
          jobOpportunity.Status = request.Status;
          jobOpportunity.RowUpdatedDate = currentDate;

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

          // Response'a ekle
          response.UpdatedJobOpportunities.Add(new JobOpportunitiesDTO
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
          });
        }

        // Değişiklikleri kaydet
        await _context.SaveChangesAsync(cancellationToken);

        response.SuccessfullyUpdated = response.UpdatedJobOpportunities.Count;
        response.Failed = response.TotalRequested - response.SuccessfullyUpdated;

        var message = $"{response.SuccessfullyUpdated} JobOpportunity başarıyla güncellendi";
        if (response.Failed > 0)
        {
          message += $", {response.Failed} işlem başarısız";
        }

        return ResultFactory.CreateSuccessResult(response, string.Join(",", request.JobOpportunityIds), null,
            "Başarılı", message, "Toplu işlem tamamlandı.");
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<BulkApproveJobOpportunitiesCommandResponse>(
            string.Join(",", request.JobOpportunityIds), null, "Hata / Sistem Hatası",
            $"Toplu JobOpportunity güncelleme işlemi sırasında hata oluştu: {ex.Message}",
            "Sistem hatası oluştu. Lütfen tekrar deneyin.");
      }
    }
  }
}
