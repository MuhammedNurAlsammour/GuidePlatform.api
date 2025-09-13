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
    public string? SubDescription { get; set; } // وصف فرعي أو مختصر

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

    // 🎯 Ana iletişim bilgileri - Primary contact information
    public int? PrimaryContactType1 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website
    public string? PrimaryContactValue1 { get; set; }  // Ana iletişim değeri 1 - Primary contact value 1
    public int? PrimaryContactType2 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website
    public string? PrimaryContactValue2 { get; set; }  // Ana iletişim değeri 2 - Primary contact value 2

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
        SubDescription = request.SubDescription,
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
        PrimaryContactType1 = request.PrimaryContactType1,
        PrimaryContactValue1 = request.PrimaryContactValue1,
        PrimaryContactType2 = request.PrimaryContactType2,
        PrimaryContactValue2 = request.PrimaryContactValue2,
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

