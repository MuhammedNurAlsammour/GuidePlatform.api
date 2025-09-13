using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.Application.Features.Commands.Base;
using O = GuidePlatform.Domain.Entities.BusinessesViewModel;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Businesses.UpdateBusinesses
{
  public class UpdateBusinessesCommandRequest : BaseCommandRequest, IRequest<TransactionResultPack<UpdateBusinessesCommandResponse>>
  {
    [Required(ErrorMessage = "Id is required")]
    [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
      ErrorMessage = "Invalid GUID format")]
    public string Id { get; set; } = string.Empty;

    // 🏢 Temel iş bilgileri - Basic business information
    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 255 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [StringLength(1000, ErrorMessage = "SubDescription cannot exceed 1000 characters")]
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
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [StringLength(20, ErrorMessage = "Mobile cannot exceed 20 characters")]
    public string? Mobile { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Website cannot exceed 500 characters")]
    public string? Website { get; set; }

    [StringLength(500, ErrorMessage = "Facebook URL cannot exceed 500 characters")]
    public string? FacebookUrl { get; set; }

    [StringLength(500, ErrorMessage = "Instagram URL cannot exceed 500 characters")]
    public string? InstagramUrl { get; set; }

    [StringLength(20, ErrorMessage = "WhatsApp cannot exceed 20 characters")]
    public string? WhatsApp { get; set; }

    [StringLength(100, ErrorMessage = "Telegram cannot exceed 100 characters")]
    public string? Telegram { get; set; }

    // 🎯 Ana iletişim bilgileri - Primary contact information
    [Range(1, 8, ErrorMessage = "Primary contact type 1 must be between 1 and 8")]
    public int? PrimaryContactType1 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website

    [StringLength(500, ErrorMessage = "Primary contact value 1 cannot exceed 500 characters")]
    public string? PrimaryContactValue1 { get; set; }  // Ana iletişim değeri 1 - Primary contact value 1

    [Range(1, 8, ErrorMessage = "Primary contact type 2 must be between 1 and 8")]
    public int? PrimaryContactType2 { get; set; }      // 1:WhatsApp, 2:Phone, 3:Mobile, 4:Email, 5:Facebook, 6:Instagram, 7:Telegram, 8:Website

    [StringLength(500, ErrorMessage = "Primary contact value 2 cannot exceed 500 characters")]
    public string? PrimaryContactValue2 { get; set; }  // Ana iletişim değeri 2 - Primary contact value 2

    // 💼 İş özellikleri - Business features
    [Range(0, 10, ErrorMessage = "Subscription type must be between 0 and 10")]
    public int? SubscriptionType { get; set; }

    public bool? IsVerified { get; set; }
    public bool? IsFeatured { get; set; }

    [StringLength(1000, ErrorMessage = "Working hours cannot exceed 1000 characters")]
    public string? WorkingHours { get; set; }

    [StringLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
    public string? Icon { get; set; }

    // 👤 Sahiplik bilgileri - Ownership information
    public Guid? OwnerId { get; set; }

    public static O Map(O entity, UpdateBusinessesCommandRequest request, ICurrentUserService currentUserService)
    {
      var (customerId, userId, updateUserId) = request.GetUpdateAuthIds(currentUserService);

      if (customerId.HasValue)
        entity.AuthCustomerId = customerId.Value;

      if (userId.HasValue)
        entity.AuthUserId = userId.Value;

      // 🏢 Temel iş bilgileri - Basic business information
      if (!string.IsNullOrWhiteSpace(request.Name))
        entity.Name = request.Name.Trim();

      if (!string.IsNullOrWhiteSpace(request.Description))
        entity.Description = request.Description.Trim();
      else if (request.Description == null)
        entity.Description = null; // Explicit null assignment

      if (!string.IsNullOrWhiteSpace(request.SubDescription))
        entity.SubDescription = request.SubDescription.Trim();
      else if (request.SubDescription == null)
        entity.SubDescription = null; // Explicit null assignment

      if (request.CategoryId.HasValue)
        entity.CategoryId = request.CategoryId.Value;

      if (request.SubCategoryId.HasValue)
        entity.SubCategoryId = request.SubCategoryId.Value;

      // 📍 Konum bilgileri - Location information
      if (request.ProvinceId.HasValue)
        entity.ProvinceId = request.ProvinceId.Value;

      if (request.CountriesId.HasValue)
        entity.CountriesId = request.CountriesId.Value;

      if (request.DistrictId.HasValue)
        entity.DistrictId = request.DistrictId.Value;

      if (!string.IsNullOrWhiteSpace(request.Address))
        entity.Address = request.Address.Trim();

      if (request.Latitude.HasValue)
        entity.Latitude = request.Latitude.Value;

      if (request.Longitude.HasValue)
        entity.Longitude = request.Longitude.Value;

      // 📞 İletişim bilgileri - Contact information
      if (!string.IsNullOrWhiteSpace(request.Phone))
        entity.Phone = request.Phone.Trim();

      if (!string.IsNullOrWhiteSpace(request.Mobile))
        entity.Mobile = request.Mobile.Trim();

      if (!string.IsNullOrWhiteSpace(request.Email))
        entity.Email = request.Email.Trim();

      if (!string.IsNullOrWhiteSpace(request.Website))
        entity.Website = request.Website.Trim();

      if (!string.IsNullOrWhiteSpace(request.FacebookUrl))
        entity.FacebookUrl = request.FacebookUrl.Trim();

      if (!string.IsNullOrWhiteSpace(request.InstagramUrl))
        entity.InstagramUrl = request.InstagramUrl.Trim();

      if (!string.IsNullOrWhiteSpace(request.WhatsApp))
        entity.WhatsApp = request.WhatsApp.Trim();

      if (!string.IsNullOrWhiteSpace(request.Telegram))
        entity.Telegram = request.Telegram.Trim();

      // 🎯 Ana iletişim bilgileri - Primary contact information
      if (request.PrimaryContactType1.HasValue)
        entity.PrimaryContactType1 = request.PrimaryContactType1.Value;

      if (!string.IsNullOrWhiteSpace(request.PrimaryContactValue1))
        entity.PrimaryContactValue1 = request.PrimaryContactValue1.Trim();

      if (request.PrimaryContactType2.HasValue)
        entity.PrimaryContactType2 = request.PrimaryContactType2.Value;

      if (!string.IsNullOrWhiteSpace(request.PrimaryContactValue2))
        entity.PrimaryContactValue2 = request.PrimaryContactValue2.Trim();

      // 💼 İş özellikleri - Business features
      if (request.SubscriptionType.HasValue)
        entity.SubscriptionType = request.SubscriptionType.Value;

      if (request.IsVerified.HasValue)
        entity.IsVerified = request.IsVerified.Value;

      if (request.IsFeatured.HasValue)
        entity.IsFeatured = request.IsFeatured.Value;

      if (!string.IsNullOrWhiteSpace(request.WorkingHours))
        entity.WorkingHours = request.WorkingHours.Trim();

      if (!string.IsNullOrWhiteSpace(request.Icon))
        entity.Icon = request.Icon.Trim();

      // 👤 Sahiplik bilgileri - Ownership information
      if (request.OwnerId.HasValue)
        entity.OwnerId = request.OwnerId.Value;

      if (updateUserId.HasValue)
      {
        entity.UpdateUserId = updateUserId.Value;
      }

      return entity;
    }
  }
}
