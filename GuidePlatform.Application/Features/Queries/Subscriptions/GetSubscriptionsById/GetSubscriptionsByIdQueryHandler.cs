using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetSubscriptionsById
{
    // Bu handler, bir subscriptions ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
    public class GetSubscriptionsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetSubscriptionsByIdQueryRequest, TransactionResultPack<GetSubscriptionsByIdQueryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetSubscriptionsByIdQueryHandler(
            IApplicationDbContext context,
            IAuthUserDetailService authUserService,
            ICurrentUserService currentUserService) : base(currentUserService, authUserService)
        {
            _context = context;
        }

        public async Task<TransactionResultPack<GetSubscriptionsByIdQueryResponse>> Handle(GetSubscriptionsByIdQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // 🎯 ID doğrulama - ID parametresi kontrolü
                if (string.IsNullOrEmpty(request.Id))
                {
                    // Eksik parametre hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetSubscriptionsByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Eksik Parametre",
                        "subscriptions ID'si belirtilmedi.",
                        "subscriptions ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
                    );
                }

                var subscriptionsId = request.GetIdAsGuid();
                if (!subscriptionsId.HasValue)
                {
                    // Geçersiz ID formatı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetSubscriptionsByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Geçersiz ID",
                        "Geçersiz subscriptions ID formatı.",
                        $"Geçersiz subscriptions ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
                    );
                }

                // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
                var authUserId = GetSafeUserId(request.AuthUserId);
                var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

                // Temel sorgu oluşturuluyor
                var baseQuery = _context.subscriptions
                    .Where(x => x.RowIsActive && !x.RowIsDeleted);

                // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
                var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
                var totalCount = await totalCountQuery
                    .Where(x => x.Id == subscriptionsId.Value)
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                // Yetkilendirme filtreleri uygulanıyor ve subscriptions çekiliyor
                var subscriptions = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
                    .Where(x => x.Id == subscriptionsId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (subscriptions == null)
                {
                    // subscriptions bulunamadı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetSubscriptionsByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / subscriptions Bulunamadı",
                        "Belirtilen ID'ye sahip subscriptions bulunamadı.",
                        $"ID '{request.Id}' ile eşleşen subscriptions bulunamadı."
                    );
                }

                // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
                var allUserIds = new List<Guid>();
                
                // Auth kullanıcı bilgileri
                if (subscriptions.AuthUserId.HasValue)
                    allUserIds.Add(subscriptions.AuthUserId.Value);
                
                // Create kullanıcı bilgileri
                if (subscriptions.CreateUserId.HasValue)
                    allUserIds.Add(subscriptions.CreateUserId.Value);
                
                // Update kullanıcı bilgileri
                if (subscriptions.UpdateUserId.HasValue)
                    allUserIds.Add(subscriptions.UpdateUserId.Value);
                
                // Tek seferde tüm kullanıcı bilgilerini al
                var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

                // 🎯 Auth kullanıcı bilgilerini al
                string? authUserName = null;
                string? authCustomerName = null;

                if (subscriptions.AuthUserId.HasValue && allUserDetails.ContainsKey(subscriptions.AuthUserId.Value))
                {
                    var userDetail = allUserDetails[subscriptions.AuthUserId.Value];
                    authUserName = userDetail.AuthUserName;
                    authCustomerName = userDetail.AuthCustomerName;
                }

                // 🎯 Create/Update kullanıcı bilgilerini al
                string? createUserName = null;
                string? updateUserName = null;

                if (subscriptions.CreateUserId.HasValue && allUserDetails.ContainsKey(subscriptions.CreateUserId.Value))
                {
                    var createUserDetail = allUserDetails[subscriptions.CreateUserId.Value];
                    createUserName = createUserDetail.AuthUserName;
                }

                if (subscriptions.UpdateUserId.HasValue && allUserDetails.ContainsKey(subscriptions.UpdateUserId.Value))
                {
                    var updateUserDetail = allUserDetails[subscriptions.UpdateUserId.Value];
                    updateUserName = updateUserDetail.AuthUserName;
                }

                // subscriptions detay DTO'su oluşturuluyor
                var subscriptionsDetail = new SubscriptionsDTO
                {
                  Id = subscriptions.Id,
                  AuthUserId = subscriptions.AuthUserId,
                  AuthCustomerId = subscriptions.AuthCustomerId,
                  AuthUserName = authUserName,      // Service'den gelen
                  AuthCustomerName = authCustomerName, // Service'den gelen
                  CreateUserName = createUserName,  // Service'den gelen
                  UpdateUserName = updateUserName,  // Service'den gelen
                  CreateUserId = subscriptions.CreateUserId,
                  UpdateUserId = subscriptions.UpdateUserId,
                  RowCreatedDate = subscriptions.RowCreatedDate,
                  RowUpdatedDate = subscriptions.RowUpdatedDate,
                  RowIsActive = subscriptions.RowIsActive,
                  RowIsDeleted = subscriptions.RowIsDeleted
                };

                // Başarılı işlem sonucu döndürülüyor
                return ResultFactory.CreateSuccessResult<GetSubscriptionsByIdQueryResponse>(
                    new GetSubscriptionsByIdQueryResponse
                    {
                        subscriptions = subscriptionsDetail,
                        TotalCount = totalCount
                    },
                    request.Id,
                    null,
                    "İşlem Başarılı",
                    "subscriptions başarıyla getirildi.",
                    $"subscriptions Id: { subscriptions.Id } başarıyla getirildi."
                );
            }
            catch (Exception ex)
            {
                // Hata durumunda hata sonucu döndürülüyor
                return ResultFactory.CreateErrorResult<GetSubscriptionsByIdQueryResponse>(
                    request.Id,
                    null,
                    "Hata / İşlem Başarısız",
                    "subscriptions getirilirken bir hata oluştu.",
                    ex.Message
                );
            }
        }
    }
}

