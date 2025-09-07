using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.PagesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Pages.CreatePages
{
  public class CreatePagesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreatePagesCommandResponse>>
  {
    // Sayfa ile ilgili temel bilgiler - Basic page information
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Slug must be between 1 and 255 characters")]
    public string Slug { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime? PublishedDate { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "article";

    public static O Map(CreatePagesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        Title = request.Title,
        Slug = request.Slug,
        Content = request.Content,
        MetaDescription = request.MetaDescription,
        MetaKeywords = request.MetaKeywords,
        IsPublished = request.IsPublished,
        PublishedDate = request.PublishedDate,
        Icon = request.Icon,
      };
    }
  }
}

