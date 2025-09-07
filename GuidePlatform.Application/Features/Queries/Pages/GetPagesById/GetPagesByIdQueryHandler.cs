using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Pages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Pages.GetPagesById
{
  // Bu handler, bir pages ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetPagesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetPagesByIdQueryRequest, TransactionResultPack<GetPagesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetPagesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetPagesByIdQueryResponse>> Handle(GetPagesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "pages ID'si belirtilmedi.",
              "pages ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var pagesId = request.GetIdAsGuid();
        if (!pagesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz pages ID formatı.",
              $"Geçersiz pages ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.pages
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == pagesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve pages çekiliyor
        var pages = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == pagesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (pages == null)
        {
          // pages bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPagesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / pages Bulunamadı",
              "Belirtilen ID'ye sahip pages bulunamadı.",
              $"ID '{request.Id}' ile eşleşen pages bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (pages.AuthUserId.HasValue)
          allUserIds.Add(pages.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (pages.CreateUserId.HasValue)
          allUserIds.Add(pages.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (pages.UpdateUserId.HasValue)
          allUserIds.Add(pages.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (pages.AuthUserId.HasValue && allUserDetails.ContainsKey(pages.AuthUserId.Value))
        {
          var userDetail = allUserDetails[pages.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (pages.CreateUserId.HasValue && allUserDetails.ContainsKey(pages.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[pages.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (pages.UpdateUserId.HasValue && allUserDetails.ContainsKey(pages.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[pages.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // pages detay DTO'su oluşturuluyor
        var pagesDetail = new PagesDTO
        {
          Id = pages.Id,
          AuthUserId = pages.AuthUserId,
          AuthCustomerId = pages.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = pages.CreateUserId,
          UpdateUserId = pages.UpdateUserId,
          RowCreatedDate = pages.RowCreatedDate,
          RowUpdatedDate = pages.RowUpdatedDate,
          RowIsActive = pages.RowIsActive,
          RowIsDeleted = pages.RowIsDeleted,
          // Sayfa özel alanları - Page specific fields
          Title = pages.Title,
          Slug = pages.Slug,
          Content = pages.Content,
          MetaDescription = pages.MetaDescription,
          MetaKeywords = pages.MetaKeywords,
          IsPublished = pages.IsPublished,
          PublishedDate = pages.PublishedDate,
          Icon = pages.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetPagesByIdQueryResponse>(
            new GetPagesByIdQueryResponse
            {
              pages = pagesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "pages başarıyla getirildi.",
            $"pages Id: {pages.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetPagesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "pages getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

