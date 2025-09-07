using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Payments;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Payments.GetPaymentsById
{
  // Bu handler, bir payments ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetPaymentsByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetPaymentsByIdQueryRequest, TransactionResultPack<GetPaymentsByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetPaymentsByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetPaymentsByIdQueryResponse>> Handle(GetPaymentsByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPaymentsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "payments ID'si belirtilmedi.",
              "payments ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var paymentsId = request.GetIdAsGuid();
        if (!paymentsId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPaymentsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz payments ID formatı.",
              $"Geçersiz payments ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.payments
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == paymentsId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve payments çekiliyor
        var payments = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == paymentsId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (payments == null)
        {
          // payments bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetPaymentsByIdQueryResponse>(
              request.Id,
              null,
              "Hata / payments Bulunamadı",
              "Belirtilen ID'ye sahip payments bulunamadı.",
              $"ID '{request.Id}' ile eşleşen payments bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (payments.AuthUserId.HasValue)
          allUserIds.Add(payments.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (payments.CreateUserId.HasValue)
          allUserIds.Add(payments.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (payments.UpdateUserId.HasValue)
          allUserIds.Add(payments.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (payments.AuthUserId.HasValue && allUserDetails.ContainsKey(payments.AuthUserId.Value))
        {
          var userDetail = allUserDetails[payments.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (payments.CreateUserId.HasValue && allUserDetails.ContainsKey(payments.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[payments.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (payments.UpdateUserId.HasValue && allUserDetails.ContainsKey(payments.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[payments.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // payments detay DTO'su oluşturuluyor
        var paymentsDetail = new PaymentsDTO
        {
          Id = payments.Id,
          AuthUserId = payments.AuthUserId,
          AuthCustomerId = payments.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = payments.CreateUserId,
          UpdateUserId = payments.UpdateUserId,
          RowCreatedDate = payments.RowCreatedDate,
          RowUpdatedDate = payments.RowUpdatedDate,
          RowIsActive = payments.RowIsActive,
          RowIsDeleted = payments.RowIsDeleted,
          // Ödeme özel alanları - Payment specific fields
          SubscriptionId = payments.SubscriptionId,
          Amount = payments.Amount,
          Currency = payments.Currency,
          PaymentMethod = payments.PaymentMethod,
          TransactionId = payments.TransactionId,
          PaymentDate = payments.PaymentDate,
          Status = payments.Status,
          Notes = payments.Notes,
          Icon = payments.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetPaymentsByIdQueryResponse>(
            new GetPaymentsByIdQueryResponse
            {
              payments = paymentsDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "payments başarıyla getirildi.",
            $"payments Id: {payments.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetPaymentsByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "payments getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

