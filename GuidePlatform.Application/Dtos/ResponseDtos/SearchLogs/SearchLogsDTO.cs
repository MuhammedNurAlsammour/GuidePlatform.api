
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs
{
  public class SearchLogsDTO : BaseResponseDTO
  {
    // Search specific fields - Arama özel alanları
    public string SearchTerm { get; set; } = string.Empty;
    public string? SearchFilters { get; set; } // JSON string
    public int? ResultsCount { get; set; }
    public DateTime? SearchDate { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string Icon { get; set; } = "search";

    // Additional computed fields - Ek hesaplanan alanlar
    public string? SearchDateFormatted { get; set; } // Formatted date for display
    public string? ResultsCountFormatted { get; set; } // Formatted results count
    public bool HasFilters => !string.IsNullOrEmpty(SearchFilters);
  }
}
