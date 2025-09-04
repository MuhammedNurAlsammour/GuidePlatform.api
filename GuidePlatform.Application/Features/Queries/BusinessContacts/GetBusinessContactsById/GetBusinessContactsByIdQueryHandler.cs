using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetBusinessContactsById
{
  // Bu handler, bir businessContacts ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetBusinessContactsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessContactsByIdQueryRequest, TransactionResultPack<GetBusinessContactsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetBusinessContactsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetBusinessContactsByIdQueryResponse>> Handle(GetBusinessContactsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessContactsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "businessContacts ID'si belirtilmedi.",
              "businessContacts ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var businessContactsId = request.GetIdAsGuid();
        if (!businessContactsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessContactsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz businessContacts ID formatı.",
              $"Geçersiz businessContacts ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.businessContacts
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == businessContactsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve businessContacts çekiliyor
        var businessContacts = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == businessContactsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (businessContacts == null)
        {
          // businessContacts bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetBusinessContactsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / businessContacts Bulunamadı",
              "Belirtilen ID'ye sahip businessContacts bulunamadı.",
              $"ID '{request.Id}' ile eşleşen businessContacts bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (businessContacts.AuthUserId.HasValue)
          allUserIds.Add(businessContacts.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (businessContacts.CreateUserId.HasValue)
          allUserIds.Add(businessContacts.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (businessContacts.UpdateUserId.HasValue)
          allUserIds.Add(businessContacts.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (businessContacts.AuthUserId.HasValue && allUserDetails.ContainsKey(businessContacts.AuthUserId.Value))
        {
          var userDetail = allUserDetails[businessContacts.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (businessContacts.CreateUserId.HasValue && allUserDetails.ContainsKey(businessContacts.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[businessContacts.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (businessContacts.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessContacts.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[businessContacts.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // businessContacts detay DTO'su oluşturuluyor
        var businessContactsDetail = new BusinessContactsDTO
        {
          Id = businessContacts.Id,
          BusinessId = businessContacts.BusinessId,
          ContactType = businessContacts.ContactType,
          ContactValue = businessContacts.ContactValue,
          IsPrimary = businessContacts.IsPrimary,
          Icon = businessContacts.Icon,
          AuthUserId = businessContacts.AuthUserId,
          AuthCustomerId = businessContacts.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = businessContacts.CreateUserId,
          UpdateUserId = businessContacts.UpdateUserId,
          RowCreatedDate = businessContacts.RowCreatedDate,
          RowUpdatedDate = businessContacts.RowUpdatedDate,
          RowIsActive = businessContacts.RowIsActive,
          RowIsDeleted = businessContacts.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetBusinessContactsByIdQueryResponse>(
            new GetBusinessContactsByIdQueryResponse
            {
              businessContacts = businessContactsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "businessContacts başarıyla getirildi.",
            $"businessContacts Id: {businessContacts.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetBusinessContactsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "businessContacts getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

