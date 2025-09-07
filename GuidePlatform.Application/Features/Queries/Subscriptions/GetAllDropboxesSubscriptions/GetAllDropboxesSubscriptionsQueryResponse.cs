using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllDropboxesSubscriptions
{
  public class GetAllDropboxesSubscriptionsQueryResponse
  {
    public List<subscriptionsDetailDto> subscriptions { get; set; } = new List<subscriptionsDetailDto>();
  }

  public class subscriptionsDetailDto
  {
    public Guid Id { get; set; }
    public Guid? AuthCustomerId { get; set; }
    public Guid? AuthUserId { get; set; }
    public string? AuthUserName { get; set; }
    public string? AuthCustomerName { get; set; }
    // Yeni özellikler - New properties
    public Guid BusinessId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int PaymentStatus { get; set; }
    public string? Icon { get; set; }
    public int Currency { get; set; }
    public int Status { get; set; }
    public int SubscriptionType { get; set; }
  }
}
