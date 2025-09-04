using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetBusinessImagesById
{
  // Bu handler, bir businessImages ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessImagesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessImagesByIdQueryRequest, TransactionResultPack<GetBusinessImagesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessImagesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessImagesByIdQueryResponse>> Handle(GetBusinessImagesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessImagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "businessImages ID'si belirtilmedi.",
              "businessImages ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessImagesId = request.GetIdAsGuid();
        if (!businessImagesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessImagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz businessImages ID formatı.",
              $"Geçersiz businessImages ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businessImages
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessImagesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve businessImages çekiliyor
        var businessImages = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessImagesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businessImages == null)
        {
          // businessImages bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessImagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / businessImages Bulunamadı",
              "Belirtilen ID'ye sahip businessImages bulunamadı.",
              $"ID '{request.Id}' ile eşleşen businessImages bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businessImages.AuthUserId.HasValue)
          allUserIds.Add(businessImages.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businessImages.CreateUserId.HasValue)
          allUserIds.Add(businessImages.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businessImages.UpdateUserId.HasValue)
          allUserIds.Add(businessImages.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businessImages.AuthUserId.HasValue && allUserDetails.ContainsKey(businessImages.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businessImages.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (businessImages.CreateUserId.HasValue && allUserDetails.ContainsKey(businessImages.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[businessImages.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (businessImages.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessImages.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[businessImages.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // businessImages detay DTO'su oluşturuluyor
        var businessImagesDetail = new BusinessImagesDTO
        {
          Id = businessImages.Id,
          BusinessId = businessImages.BusinessId,
          Photo = businessImages.Photo != null ? Convert.ToBase64String(businessImages.Photo) : null,
          Thumbnail = businessImages.Thumbnail != null ? Convert.ToBase64String(businessImages.Thumbnail) : null,
          PhotoContentType = businessImages.PhotoContentType,
          AltText = businessImages.AltText,
          IsPrimary = businessImages.IsPrimary,
          SortOrder = businessImages.SortOrder,
          Icon = businessImages.Icon,
          ImageType = businessImages.ImageType,
          AuthUserId = businessImages.AuthUserId,
          AuthCustomerId = businessImages.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businessImages.CreateUserId,
          UpdateUserId = businessImages.UpdateUserId,
          RowCreatedDate = businessImages.RowCreatedDate,
          RowUpdatedDate = businessImages.RowUpdatedDate,
          RowIsActive = businessImages.RowIsActive,
          RowIsDeleted = businessImages.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessImagesByIdQueryResponse>(
            new GetBusinessImagesByIdQueryResponse
            {
              businessImages = businessImagesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "businessImages başarıyla getirildi.",
            $"businessImages Id: {businessImages.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessImagesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "businessImages getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

