using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;

namespace GuidePlatform.API.Configurations
{
  /// <summary>
  /// OData konfigürasyonu - Business Reviews için gelişmiş filtreleme ve sorgulama
  /// </summary>
  public static class ODataConfiguration
  {
    /// <summary>
    /// OData servislerini kaydet
    /// </summary>
    public static IServiceCollection AddODataServices(this IServiceCollection services)
    {
      services.AddControllers()
          .AddOData(options => options
              .Select()
              .Filter()
              .OrderBy()
              .Expand()
              .Count()
              .SetMaxTop(100)
              .AddRouteComponents("odata", GetEdmModel())
              .RouteOptions.EnableKeyInParenthesis = false);

      return services;
    }

    /// <summary>
    /// EDM Model oluştur - Entity Data Model
    /// </summary>
    private static IEdmModel GetEdmModel()
    {
      var builder = new ODataConventionModelBuilder();

      // OData konfigürasyonu - string değerler için esnek quote desteği
      builder.EnableLowerCamelCase();

      // BusinessReviews entity'sini OData'ya eklehttp://localhost:5263/api/BusinessReviews/odata?$filter=BusinessName eq 'Aleppo'
      var businessReviewsEntitySet = builder.EntitySet<BusinessReviewsDTO>("BusinessReviews");

      // BusinessReviews için özel filtreleme seçenekleri
      var businessReviewsEntityType = businessReviewsEntitySet.EntityType;

      // Temel alanlar
      businessReviewsEntityType.HasKey(x => x.Id);

      // GUID alanları - OData için özel konfigürasyon
      businessReviewsEntityType.Property(x => x.BusinessId).IsRequired();
      businessReviewsEntityType.Property(x => x.ReviewerId).IsRequired();
      businessReviewsEntityType.Property(x => x.AuthUserId);
      businessReviewsEntityType.Property(x => x.AuthCustomerId);
      businessReviewsEntityType.Property(x => x.CreateUserId);
      businessReviewsEntityType.Property(x => x.UpdateUserId);

      // String alanları
      businessReviewsEntityType.Property(x => x.BusinessName);
      businessReviewsEntityType.Property(x => x.ReviewerName);
      businessReviewsEntityType.Property(x => x.Comment);
      businessReviewsEntityType.Property(x => x.Icon);
      businessReviewsEntityType.Property(x => x.AuthUserName);
      businessReviewsEntityType.Property(x => x.AuthCustomerName);
      businessReviewsEntityType.Property(x => x.CreateUserName);
      businessReviewsEntityType.Property(x => x.UpdateUserName);

      // Numeric alanları
      businessReviewsEntityType.Property(x => x.Rating).IsRequired();

      // Boolean alanları
      businessReviewsEntityType.Property(x => x.IsVerified);
      businessReviewsEntityType.Property(x => x.IsApproved);
      businessReviewsEntityType.Property(x => x.RowIsActive);
      businessReviewsEntityType.Property(x => x.RowIsDeleted);

      // DateTime alanları
      businessReviewsEntityType.Property(x => x.RowCreatedDate);
      businessReviewsEntityType.Property(x => x.RowUpdatedDate);

      // Özel fonksiyonlar ekle
      AddCustomFunctions(builder);

      return builder.GetEdmModel();
    }

    /// <summary>
    /// Özel OData fonksiyonları ekle
    /// </summary>
    private static void AddCustomFunctions(ODataConventionModelBuilder builder)
    {
      // İş yeri ID'sine göre yorumları getir
      var getReviewsByBusinessFunction = builder.Function("GetReviewsByBusiness");
      getReviewsByBusinessFunction.Parameter<Guid>("businessId");
      getReviewsByBusinessFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");

      // Kullanıcı ID'sine göre yorumları getir
      var getReviewsByUserFunction = builder.Function("GetReviewsByUser");
      getReviewsByUserFunction.Parameter<Guid>("userId");
      getReviewsByUserFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");

      // Rating aralığına göre yorumları getir
      var getReviewsByRatingRangeFunction = builder.Function("GetReviewsByRatingRange");
      getReviewsByRatingRangeFunction.Parameter<int>("minRating");
      getReviewsByRatingRangeFunction.Parameter<int>("maxRating");
      getReviewsByRatingRangeFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");

      // Onaylanmış yorumları getir
      var getApprovedReviewsFunction = builder.Function("GetApprovedReviews");
      getApprovedReviewsFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");

      // Doğrulanmış yorumları getir
      var getVerifiedReviewsFunction = builder.Function("GetVerifiedReviews");
      getVerifiedReviewsFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");

      // GUID string olarak al
      var getReviewsByBusinessIdStringFunction = builder.Function("GetReviewsByBusinessIdString");
      getReviewsByBusinessIdStringFunction.Parameter<string>("businessId");
      getReviewsByBusinessIdStringFunction.ReturnsCollectionFromEntitySet<BusinessReviewsDTO>("BusinessReviews");
    }
  }
}
