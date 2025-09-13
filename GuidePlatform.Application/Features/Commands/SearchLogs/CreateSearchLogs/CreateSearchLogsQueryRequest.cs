using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net;
using O = GuidePlatform.Domain.Entities.SearchLogsViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.SearchLogs.CreateSearchLogs
{
  public class CreateSearchLogsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateSearchLogsCommandResponse>>
  {
    // Search specific fields - Arama özel alanları
    [Required(ErrorMessage = "Search term is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Search term must be between 1 and 255 characters")]
    public string SearchTerm { get; set; } = string.Empty;

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
    public string Icon { get; set; } = "search";

    public static O Map(CreateSearchLogsCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        // Auth bilgilerini sadece geçerli olduğunda ata - Assign auth info only when valid
        AuthCustomerId = customerId, // Null olabilir - Can be null
        AuthUserId = userId, // Null olabilir - Can be null
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        SearchTerm = request.SearchTerm,
        SearchFilters = request.SearchFilters,
        ResultsCount = request.ResultsCount,
        SearchDate = request.SearchDate ?? DateTime.UtcNow, // Default to current time if not provided
        IpAddress = !string.IsNullOrWhiteSpace(request.IpAddress) ? IPAddress.Parse(request.IpAddress) : null,
        UserAgent = request.UserAgent,
        Icon = request.Icon,
      };
    }
  }
}

