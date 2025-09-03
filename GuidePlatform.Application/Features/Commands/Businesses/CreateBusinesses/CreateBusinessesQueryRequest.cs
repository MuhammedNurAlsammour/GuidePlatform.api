using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using O = GuidePlatform.Domain.Entities.BusinessesViewModel;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;


namespace GuidePlatform.Application.Features.Commands.Businesses.CreateBusinesses
{
  public class CreateBusinessesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<CreateBusinessesCommandResponse>>
  {

    // 🏢 Temel iş bilgileri - Basic business information
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }

    // 📍 Konum bilgileri - Location information
    public Guid? ProvinceId { get; set; }
    public Guid? CountriesId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // 📞 İletişim bilgileri - Contact information
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WhatsApp { get; set; }
    public string? Telegram { get; set; }

    // 💼 İş özellikleri - Business features
    public int SubscriptionType { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public string? WorkingHours { get; set; }
    public string Icon { get; set; } = "business";

    // 👤 Sahiplik bilgileri - Ownership information
    public Guid? OwnerId { get; set; }

    public static O Map(CreateBusinessesCommandRequest request, ICurrentUserService currentUserService)
    {
      // 🎯 Yeni otomatik auth bilgileri alma method'unu kullan - Use new automatic auth info retrieval method
      var (customerId, userId, createUserId, updateUserId) = request.GetCreateAuthIds(currentUserService);

      return new()
      {
        AuthCustomerId = customerId,
        AuthUserId = userId,
        CreateUserId = createUserId, // 🎯 Otomatik token'dan alınan CreateUserId - CreateUserId from automatic token
        Name = request.Name,
        Description = request.Description,
        CategoryId = request.CategoryId,
        SubCategoryId = request.SubCategoryId,
        ProvinceId = request.ProvinceId,
        CountriesId = request.CountriesId,
        DistrictId = request.DistrictId,
        Address = request.Address,
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        Phone = request.Phone,
        Mobile = request.Mobile,
        Email = request.Email,
        Website = request.Website,
        FacebookUrl = request.FacebookUrl,
        InstagramUrl = request.InstagramUrl,
        WhatsApp = request.WhatsApp,
        Telegram = request.Telegram,
        SubscriptionType = request.SubscriptionType,
        IsVerified = request.IsVerified,
        IsFeatured = request.IsFeatured,
        WorkingHours = request.WorkingHours,
        Icon = request.Icon,
        OwnerId = request.OwnerId
      };
    }
  }
}

