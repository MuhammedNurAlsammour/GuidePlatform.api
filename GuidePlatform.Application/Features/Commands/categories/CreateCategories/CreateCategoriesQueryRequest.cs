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
    public string Name { get; set; }

    public string? Description { get; set; }


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
      };
    }
  }
}

