using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.SearchLogsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.SearchLogs.UpdateSearchLogs
{
  public class UpdateSearchLogsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateSearchLogsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Search specific fields - Arama özel alanları
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Search term must be between 1 and 255 characters")]
    public string? SearchTerm { get; set; }

    [StringLength(4000, ErrorMessage = "Search filters cannot exceed 4000 characters")]
    public string? SearchFilters { get; set; } // JSON string

    [Range(0, int.MaxValue, ErrorMessage = "Results count must be a positive number")]
    public int? ResultsCount { get; set; }

    public DateTime? SearchDate { get; set; }

    [StringLength(45, ErrorMessage = "IP address cannot exceed 45 characters")]
    public string? IpAddress { get; set; }

    [StringLength(1000, ErrorMessage = "User agent cannot exceed 1000 characters")]
    public string? UserAgent { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateSearchLogsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Update search specific fields - Arama özel alanlarını güncelle
      if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        entity.SearchTerm = request.SearchTerm.Trim();

      if (request.SearchFilters != null)
        entity.SearchFilters = request.SearchFilters.Trim();
      else if (request.SearchFilters == null)
        entity.SearchFilters = null; // Explicit null assignment

      if (request.ResultsCount.HasValue)
        entity.ResultsCount = request.ResultsCount.Value;

      if (request.SearchDate.HasValue)
        entity.SearchDate = request.SearchDate.Value;

      if (!string.IsNullOrWhiteSpace(request.IpAddress))
        entity.IpAddress = IPAddress.Parse(request.IpAddress.Trim());
      else if (request.IpAddress == null)
        entity.IpAddress = null;

      if (!string.IsNullOrWhiteSpace(request.UserAgent))
        entity.UserAgent = request.UserAgent.Trim();
      else if (request.UserAgent == null)
        entity.UserAgent = null;

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
