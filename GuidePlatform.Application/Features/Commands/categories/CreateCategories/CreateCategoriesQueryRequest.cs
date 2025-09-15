using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.CategoriesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.categories.CreateCategories
{
  public class CreateCategoriesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateCategoriesCommandResponse>>
  {

    // 🎯 Validation örnekleri - ihtiyaca göre düzenlenebilir
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Üst kategori kimliği (alt kategoriler için)
    /// </summary>
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string? ParentId { get; set; }

    /// <summary>
    /// Kategori simgesi (Material Icons)
    /// </summary>
    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string Icon { get; set; } = "category";

    /// <summary>
    /// Sıralama düzeni
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "SortOrder must be a positive number")]
    public int SortOrder { get; set; } = 0;


    public static O Map(CreateCategoriesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - Otomatik token'dan alınan CreateUserId
        Name = request.Name,
        Description = request.Description,
        ParentId = !string.IsNullOrWhiteSpace(request.ParentId) ? Guid.Parse(request.ParentId) : null,
        Icon = request.Icon,
        SortOrder = request.SortOrder,
      };
    }
  }
}

