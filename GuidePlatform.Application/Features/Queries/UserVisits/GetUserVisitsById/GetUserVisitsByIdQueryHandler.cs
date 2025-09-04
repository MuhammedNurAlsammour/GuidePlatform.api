using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetUserVisitsById
{
  // Bu handler, bir userVisits ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetUserVisitsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserVisitsByIdQueryRequest, TransactionResultPack<GetUserVisitsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetUserVisitsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetUserVisitsByIdQueryResponse>> Handle(GetUserVisitsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserVisitsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "userVisits ID'si belirtilmedi.",
              "userVisits ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var userVisitsId = request.GetIdAsGuid();
        if (!userVisitsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserVisitsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz userVisits ID formatı.",
              $"Geçersiz userVisits ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.userVisits
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == userVisitsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve userVisits çekiliyor
        var userVisits = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == userVisitsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (userVisits == null)
        {
          // userVisits bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetUserVisitsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / userVisits Bulunamadı",
              "Belirtilen ID'ye sahip userVisits bulunamadı.",
              $"ID '{request.Id}' ile eşleşen userVisits bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (userVisits.AuthUserId.HasValue)
          allUserIds.Add(userVisits.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (userVisits.CreateUserId.HasValue)
          allUserIds.Add(userVisits.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (userVisits.UpdateUserId.HasValue)
          allUserIds.Add(userVisits.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (userVisits.AuthUserId.HasValue && allUserDetails.ContainsKey(userVisits.AuthUserId.Value))
        {
          var userDetail = allUserDetails[userVisits.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (userVisits.CreateUserId.HasValue && allUserDetails.ContainsKey(userVisits.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[userVisits.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (userVisits.UpdateUserId.HasValue && allUserDetails.ContainsKey(userVisits.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[userVisits.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // userVisits detay DTO'su oluşturuluyor
        var userVisitsDetail = new UserVisitsDTO
        {
          Id = userVisits.Id,
          BusinessId = userVisits.BusinessId,
          VisitDate = userVisits.VisitDate,
          VisitType = userVisits.VisitType,
          Icon = userVisits.Icon,
          AuthUserId = userVisits.AuthUserId,
          AuthCustomerId = userVisits.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = userVisits.CreateUserId,
          UpdateUserId = userVisits.UpdateUserId,
          RowCreatedDate = userVisits.RowCreatedDate,
          RowUpdatedDate = userVisits.RowUpdatedDate,
          RowIsActive = userVisits.RowIsActive,
          RowIsDeleted = userVisits.RowIsDeleted
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetUserVisitsByIdQueryResponse>(
            new GetUserVisitsByIdQueryResponse
            {
              userVisits = userVisitsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "userVisits başarıyla getirildi.",
            $"userVisits Id: {userVisits.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetUserVisitsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "userVisits getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

