using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.JobSeekersViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Domain.Enums;


namespace GuidePlatform.Application.Features.Commands.JobSeekers.CreateJobSeekers
{
  public class CreateJobSeekersCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateJobSeekersCommandResponse>>
  {
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string? BusinessId { get; set; }

    [Required(ErrorMessage = "FullName gerekli")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "FullName 1 ile 255 karakter arasında olmalı")]
    public string FullName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(20, ErrorMessage = "Phone 20 karakteri geçemez")]
    public string? Phone { get; set; }

    public int Duration { get; set; } = 0;

    public bool IsSponsored { get; set; } = false;

    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string? ProvinceId { get; set; }

    public JobSeekerStatus Status { get; set; } = JobSeekerStatus.Pending;


    public static O Map(CreateJobSeekersCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = !string.IsNullOrWhiteSpace(request.BusinessId) ? Guid.Parse(request.BusinessId) : (Guid?)null,
        FullName = request.FullName,
        Description = request.Description,
        Phone = request.Phone,
        Duration = request.Duration,
        IsSponsored = request.IsSponsored,
        ProvinceId = !string.IsNullOrWhiteSpace(request.ProvinceId) ? Guid.Parse(request.ProvinceId) : null,
        Status = request.Status,
      };
    }
  }
}

