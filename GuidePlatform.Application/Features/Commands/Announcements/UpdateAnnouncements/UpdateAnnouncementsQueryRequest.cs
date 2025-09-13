using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.AnnouncementsViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Announcements.UpdateAnnouncements
{
  public class UpdateAnnouncementsCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateAnnouncementsCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Duyuru ile ilgili güncellenebilir alanlar - Updatable announcement fields
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10")]
    public int? Priority { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedDate { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdateAnnouncementsCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Duyuru alanlarını güncelle - Update announcement fields
      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Content))
        entity.Content = request.Content.Trim();
      else if (request.Content == null)
        entity.Content = null;

      if (request.Priority.HasValue)
        entity.Priority = request.Priority.Value;

      if (request.IsPublished.HasValue)
        entity.IsPublished = request.IsPublished.Value;

      if (request.PublishedDate.HasValue)
        entity.PublishedDate = request.PublishedDate.Value;
      else if (request.PublishedDate == null)
        entity.PublishedDate = null;

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
