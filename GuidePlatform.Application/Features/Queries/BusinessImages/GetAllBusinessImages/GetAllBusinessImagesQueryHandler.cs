using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllBusinessImages
{
  public class GetAllBusinessImagesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessImagesQueryRequest, TransactionResultPack<GetAllBusinessImagesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessImagesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessImagesQueryResponse>> Handle(GetAllBusinessImagesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessImages
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyBusinessImagesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessImagess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessImagess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessImagess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessImagess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessImagesDetails = new List<BusinessImagesDTO>();  // 🎯 businessImagesDTO listesi oluştur

        foreach (var businessImages in businessImagess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businessImages.AuthUserId.HasValue && allUserDetails.ContainsKey(businessImages.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businessImages.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var businessImagesDetail = new BusinessImagesDTO
          {
            Id = businessImages.Id,
            BusinessId = businessImages.BusinessId,
            // Yeni sistem: URL'leri kullan - New system: Use URLs
            PhotoUrl = businessImages.PhotoUrl,
            ThumbnailUrl = businessImages.ThumbnailUrl,
            // Eski sistem: Base64'ü koru (geriye dönük uyumluluk için) - Old system: Keep Base64 (for backward compatibility)
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
            CreateUserId = businessImages.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessImages.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessImages.RowCreatedDate,
            RowUpdatedDate = businessImages.RowUpdatedDate,
            RowIsActive = businessImages.RowIsActive,
            RowIsDeleted = businessImages.RowIsDeleted
          };

          businessImagesDetails.Add(businessImagesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessImagesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessImagesQueryResponse
            {
              TotalCount = totalCount,
              businessImages = businessImagesDetails  // 🎯 businessImagesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessImages başarıyla getirildi.",
            $"businessImagess.Count businessImages başarıyla getirildi."  // 🎯 businessImages sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessImagesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessImages getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// İşletme resim filtrelerini uygular - Applies business images filters
    /// </summary>
    private IQueryable<BusinessImagesViewModel> ApplyBusinessImagesFilters(
        IQueryable<BusinessImagesViewModel> query,
        GetAllBusinessImagesQueryRequest request)
    {
      // 🏢 İşletme bilgileri filtreleri - Business information filters
      if (request.BusinessId.HasValue)
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);

      // 🖼️ Resim bilgileri filtreleri - Image information filters
      if (request.IsPrimary.HasValue)
        query = query.Where(x => x.IsPrimary == request.IsPrimary.Value);

      if (request.SortOrder.HasValue)
        query = query.Where(x => x.SortOrder == request.SortOrder.Value);

      if (request.ImageType.HasValue)
        query = query.Where(x => x.ImageType == request.ImageType.Value);

      return query;
    }
  }
}
