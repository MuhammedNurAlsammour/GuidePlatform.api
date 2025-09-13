using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.JobOpportunitiesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Enums;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunities
{
  public class UpdateJobOpportunitiesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateJobOpportunitiesCommandResponse>>
  {
    [Required(ErrorMessage = "Id gerekli")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Geçersiz GUID formatı")]
    public string Id { get; set; } = string.Empty;

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

    public JobOpportunityStatus? Status { get; set; }

    public static O Map(O entity, UpdateJobOpportunitiesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      if (!string.IsNullOrWhiteSpace(request.BusinessId))
        entity.BusinessId = Guid.Parse(request.BusinessId);

      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Description))
        entity.Description = request.Description.Trim();

      if (!string.IsNullOrWhiteSpace(request.Phone))
        entity.Phone = request.Phone.Trim();
      else if (request.Phone == null)
        entity.Phone = null;

      entity.Duration = request.Duration;
      entity.IsSponsored = request.IsSponsored;

      if (request.Status.HasValue)
        entity.Status = request.Status.Value;

      if (!string.IsNullOrWhiteSpace(request.ProvinceId))
        entity.ProvinceId = Guid.Parse(request.ProvinceId);
      else if (request.ProvinceId == null)
        entity.ProvinceId = null;

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
