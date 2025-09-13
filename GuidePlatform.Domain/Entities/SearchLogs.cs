using GuidePlatform.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuidePlatform.Domain.Entities
{
  public class SearchLogsViewModel : BaseEntity
  {
    // Search specific fields - Arama özel alanları
    public string SearchTerm { get; set; } = string.Empty;
    public string? SearchFilters { get; set; } // JSON string olarak saklanacak - Will be stored as JSON string
    public int? ResultsCount { get; set; }
    public DateTime? SearchDate { get; set; }
    [NotMapped] // Temporary fix until EF cache is cleared - Geçici çözüم, EF cache temizlenene kadar
    public IPAddress? IpAddress
    {
      get => !string.IsNullOrWhiteSpace(IpAddressString) ? IPAddress.Parse(IpAddressString) : null;
      set => IpAddressString = value?.ToString();
    }

    // Temporary string property for EF mapping - EF mapping için geçici string property
    public string? IpAddressString { get; set; }

    public string? UserAgent { get; set; }
    public string Icon { get; set; } = "search";
  }
}