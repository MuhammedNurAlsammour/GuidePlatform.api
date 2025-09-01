using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using Karmed.External.Auth.Library.Services;
using O = GuidePlatform.Domain.Entities.CategoriesViewModel;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.categories.UpdateCategories
{
  public class UpdateCategoriesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateCategoriesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", 
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // 🎯 Validation örnekleri - ihtiyaca göre düzenlenebilir - Validation örnekleri - ihtiyaca göre düzenlenebilir
    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 255 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public static O Map(O entity, UpdateCategoriesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Yeni otomatik auth bilgileri alma method'unu kullan
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      // 🎯 Null check ve güvenli atama - Null check ve güvenli atama
      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;
      
      if (userId.HasValue)
        entity.AuthUserId = userId.Value;
      
      if (!string.IsNullOrWhiteSpace(request.Name))
        entity.Name = request.Name.Trim();
      
      if (!string.IsNullOrWhiteSpace(request.Description))
        entity.Description = request.Description.Trim();
      else if (request.Description == null)
        entity.Description = null; // Explicit null assignment

      // 🎯 Otomatik token'dan alınan UpdateUserId'yi ata - Otomatik token'dan alınan UpdateUserId'yi ata
      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value; // 🎯 Güncelleme yapan kullanıcıyı otomatik token'dan al - Güncelleme yapan kullanıcıyı otomatik token'dan al
      }

      return entity;
    }
  }
}
