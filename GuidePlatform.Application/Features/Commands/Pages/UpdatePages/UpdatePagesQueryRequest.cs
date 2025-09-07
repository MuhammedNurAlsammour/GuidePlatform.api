using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.PagesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Pages.UpdatePages
{
  public class UpdatePagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdatePagesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // Sayfa ile ilgili güncellenebilir alanlar - Updatable page fields
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string? Title { get; set; }

    [StringLength(255, MinimumLength = 1, ErrorMessage = "Slug must be between 1 and 255 characters")]
    public string? Slug { get; set; }

    public string? Content { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedDate { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    public static O Map(O entity, UpdatePagesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // Sayfa alanlarını güncelle - Update page fields
      if (!string.IsNullOrWhiteSpace(request.Title))
        entity.Title = request.Title.Trim();

      if (!string.IsNullOrWhiteSpace(request.Slug))
        entity.Slug = request.Slug.Trim();

      if (!string.IsNullOrWhiteSpace(request.Content))
        entity.Content = request.Content.Trim();
      else if (request.Content == null)
        entity.Content = null;

      if (!string.IsNullOrWhiteSpace(request.MetaDescription))
        entity.MetaDescription = request.MetaDescription.Trim();
      else if (request.MetaDescription == null)
        entity.MetaDescription = null;

      if (!string.IsNullOrWhiteSpace(request.MetaKeywords))
        entity.MetaKeywords = request.MetaKeywords.Trim();
      else if (request.MetaKeywords == null)
        entity.MetaKeywords = null;

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
