using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.JobOpportunitiesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Domain.Enums;


namespace GuidePlatform.Application.Features.Commands.JobOpportunities.CreateJobOpportunities
{
  public class CreateJobOpportunitiesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateJobOpportunitiesCommandResponse>>
  {
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string BusinessId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title gerekli")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title 1 ile 255 karakter arasında olmalı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description gerekli")]
    public string Description { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Phone 20 karakteri geçemez")]
    public string? Phone { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Duration 0 veya daha büyük olmalı")]
    public int Duration { get; set; } = 0;

    public bool IsSponsored { get; set; } = false;

    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string? ProvinceId { get; set; }

    public JobOpportunityStatus Status { get; set; } = JobOpportunityStatus.Pending;


    public static O Map(CreateJobOpportunitiesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId
        BusinessId = Guid.Parse(request.BusinessId),
        Title = request.Title,
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

