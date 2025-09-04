using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetBusinessServicesById
{
    // Bu handler, bir businessServices ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
    public class GetBusinessServicesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBusinessServicesByIdQueryRequest, TransactionResultPack<GetBusinessServicesByIdQueryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetBusinessServicesByIdQueryHandler(
            IApplicationDbContext context,
            IAuthUserDetailService authUserService,
            ICurrentUserService currentUserService) : base(currentUserService, authUserService)
        {
            _context = context;
        }

        public async Task<TransactionResultPack<GetBusinessServicesByIdQueryResponse>> Handle(GetBusinessServicesByIdQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // 🎯 ID doğrulama - ID parametresi kontrolü
                if (string.IsNullOrEmpty(request.Id))
                {
                    // Eksik parametre hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetBusinessServicesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Eksik Parametre",
                        "businessServices ID'si belirtilmedi.",
                        "businessServices ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
                    );
                }

                var businessServicesId = request.GetIdAsGuid();
                if (!businessServicesId.HasValue)
                {
                    // Geçersiz ID formatı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetBusinessServicesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Geçersiz ID",
                        "Geçersiz businessServices ID formatı.",
                        $"Geçersiz businessServices ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
                    );
                }

                // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
                var authUserId = GetSafeUserId(request.AuthUserId);
                var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

                // Temel sorgu oluşturuluyor
                var baseQuery = _context.businessServices
                    .Where(x => x.RowIsActive && !x.RowIsDeleted);

                // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
                var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
                var totalCount = await totalCountQuery
                    .Where(x => x.Id == businessServicesId.Value)
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                // Yetkilendirme filtreleri uygulanıyor ve businessServices çekiliyor
                var businessServices = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
                    .Where(x => x.Id == businessServicesId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (businessServices == null)
                {
                    // businessServices bulunamadı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetBusinessServicesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / businessServices Bulunamadı",
                        "Belirtilen ID'ye sahip businessServices bulunamadı.",
                        $"ID '{request.Id}' ile eşleşen businessServices bulunamadı."
                    );
                }

                // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
                var allUserIds = new List<Guid>();
                
                // Auth kullanıcı bilgileri
                if (businessServices.AuthUserId.HasValue)
                    allUserIds.Add(businessServices.AuthUserId.Value);
                
                // Create kullanıcı bilgileri
                if (businessServices.CreateUserId.HasValue)
                    allUserIds.Add(businessServices.CreateUserId.Value);
                
                // Update kullanıcı bilgileri
                if (businessServices.UpdateUserId.HasValue)
                    allUserIds.Add(businessServices.UpdateUserId.Value);
                
                // Tek seferde tüm kullanıcı bilgilerini al
                var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

                // 🎯 Auth kullanıcı bilgilerini al
                string? authUserName = null;
                string? authCustomerName = null;

                if (businessServices.AuthUserId.HasValue && allUserDetails.ContainsKey(businessServices.AuthUserId.Value))
                {
                    var userDetail = allUserDetails[businessServices.AuthUserId.Value];
                    authUserName = userDetail.AuthUserName;
                    authCustomerName = userDetail.AuthCustomerName;
                }

                // 🎯 Create/Update kullanıcı bilgilerini al
                string? createUserName = null;
                string? updateUserName = null;

                if (businessServices.CreateUserId.HasValue && allUserDetails.ContainsKey(businessServices.CreateUserId.Value))
                {
                    var createUserDetail = allUserDetails[businessServices.CreateUserId.Value];
                    createUserName = createUserDetail.AuthUserName;
                }

                if (businessServices.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessServices.UpdateUserId.Value))
                {
                    var updateUserDetail = allUserDetails[businessServices.UpdateUserId.Value];
                    updateUserName = updateUserDetail.AuthUserName;
                }

                // businessServices detay DTO'su oluşturuluyor
                var businessServicesDetail = new BusinessServicesDTO
                {
                  Id = businessServices.Id,
                  BusinessId = businessServices.BusinessId,
                  ServiceName = businessServices.ServiceName,
                  ServiceDescription = businessServices.ServiceDescription,
                  Price = businessServices.Price,
                  Currency = businessServices.Currency,
                  IsAvailable = businessServices.IsAvailable,
                  Icon = businessServices.Icon,
                  AuthUserId = businessServices.AuthUserId,
                  AuthCustomerId = businessServices.AuthCustomerId,
                  AuthUserName = authUserName,      // Service'den gelen
                  AuthCustomerName = authCustomerName, // Service'den gelen
                  CreateUserName = createUserName,  // Service'den gelen
                  UpdateUserName = updateUserName,  // Service'den gelen
                  CreateUserId = businessServices.CreateUserId,
                  UpdateUserId = businessServices.UpdateUserId,
                  RowCreatedDate = businessServices.RowCreatedDate,
                  RowUpdatedDate = businessServices.RowUpdatedDate,
                  RowIsActive = businessServices.RowIsActive,
                  RowIsDeleted = businessServices.RowIsDeleted
                };

                // Başarılı işlem sonucu döndürülüyor
                return ResultFactory.CreateSuccessResult<GetBusinessServicesByIdQueryResponse>(
                    new GetBusinessServicesByIdQueryResponse
                    {
                        businessServices = businessServicesDetail,
                        TotalCount = totalCount
                    },
                    request.Id,
                    null,
                    "İşlem Başarılı",
                    "businessServices başarıyla getirildi.",
                    $"businessServices Id: { businessServices.Id } başarıyla getirildi."
                );
            }
            catch (Exception ex)
            {
                // Hata durumunda hata sonucu döndürülüyor
                return ResultFactory.CreateErrorResult<GetBusinessServicesByIdQueryResponse>(
                    request.Id,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessServices getirilirken bir hata oluştu.",
                    ex.Message
                );
            }
        }
    }
}

