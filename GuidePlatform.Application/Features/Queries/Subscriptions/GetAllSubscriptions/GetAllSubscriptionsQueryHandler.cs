using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllSubscriptions
{
  public class GetAllSubscriptionsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllSubscriptionsQueryRequest, TransactionResultPack<GetAllSubscriptionsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllSubscriptionsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllSubscriptionsQueryResponse>> Handle(GetAllSubscriptionsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.subscriptions
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplySubscriptionsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var subscriptionss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = subscriptionss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = subscriptionss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = subscriptionss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var subscriptionsDetails = new List<SubscriptionsDTO>();  // 🎯 subscriptionsDTO listesi oluştur

        foreach (var subscriptions in subscriptionss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (subscriptions.AuthUserId.HasValue && allUserDetails.ContainsKey(subscriptions.AuthUserId.Value))
          {
            var userDetail = allUserDetails[subscriptions.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var subscriptionsDetail = new SubscriptionsDTO
          {
            Id = subscriptions.Id,
            AuthUserId = subscriptions.AuthUserId,
            AuthCustomerId = subscriptions.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = subscriptions.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = subscriptions.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = subscriptions.RowCreatedDate,
            RowUpdatedDate = subscriptions.RowUpdatedDate,
            RowIsActive = subscriptions.RowIsActive,
            RowIsDeleted = subscriptions.RowIsDeleted,
            // Yeni özellikler - New properties
            BusinessId = subscriptions.BusinessId,
            StartDate = subscriptions.StartDate,
            EndDate = subscriptions.EndDate,
            Amount = subscriptions.Amount,
            PaymentStatus = subscriptions.PaymentStatus,
            Icon = subscriptions.Icon,
            Currency = subscriptions.Currency,
            Status = subscriptions.Status,
            SubscriptionType = subscriptions.SubscriptionType
          };

          subscriptionsDetails.Add(subscriptionsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllSubscriptionsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllSubscriptionsQueryResponse
            {
              TotalCount = totalCount,
              subscriptions = subscriptionsDetails  // 🎯 subscriptionsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "subscriptions başarıyla getirildi.",
            $"subscriptionss.Count subscriptions başarıyla getirildi."  // 🎯 subscriptions sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllSubscriptionsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "subscriptions getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Subscriptions filtrelerini uygular - Applies Subscriptions filters
    /// </summary>
    private IQueryable<SubscriptionsViewModel> ApplySubscriptionsFilters(
        IQueryable<SubscriptionsViewModel> query,
        GetAllSubscriptionsQueryRequest request)
    {
      // 🔍 İş kimliği filtresi - Business ID filter
      if (request.BusinessId.HasValue)
      {
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);
      }

      // 🔍 Başlangıç tarihi filtresi - Start date filter
      if (request.StartDateFrom.HasValue)
      {
        query = query.Where(x => x.StartDate >= request.StartDateFrom.Value);
      }

      if (request.StartDateTo.HasValue)
      {
        query = query.Where(x => x.StartDate <= request.StartDateTo.Value);
      }

      // 🔍 Bitiş tarihi filtresi - End date filter
      if (request.EndDateFrom.HasValue)
      {
        query = query.Where(x => x.EndDate >= request.EndDateFrom.Value);
      }

      if (request.EndDateTo.HasValue)
      {
        query = query.Where(x => x.EndDate <= request.EndDateTo.Value);
      }

      // 🔍 Ödeme durumu filtresi - Payment status filter
      if (request.PaymentStatus.HasValue)
      {
        query = query.Where(x => x.PaymentStatus == request.PaymentStatus.Value);
      }

      // 🔍 Para birimi filtresi - Currency filter
      if (request.Currency.HasValue)
      {
        query = query.Where(x => x.Currency == request.Currency.Value);
      }

      // 🔍 Durum filtresi - Status filter
      if (request.Status.HasValue)
      {
        query = query.Where(x => x.Status == request.Status.Value);
      }

      // 🔍 Abonelik türü filtresi - Subscription type filter
      if (request.SubscriptionType.HasValue)
      {
        query = query.Where(x => x.SubscriptionType == request.SubscriptionType.Value);
      }

      // 🔍 Tutar aralığı filtresi - Amount range filter
      if (request.AmountFrom.HasValue)
      {
        query = query.Where(x => x.Amount >= request.AmountFrom.Value);
      }

      if (request.AmountTo.HasValue)
      {
        query = query.Where(x => x.Amount <= request.AmountTo.Value);
      }

      return query;
    }
  }
}
