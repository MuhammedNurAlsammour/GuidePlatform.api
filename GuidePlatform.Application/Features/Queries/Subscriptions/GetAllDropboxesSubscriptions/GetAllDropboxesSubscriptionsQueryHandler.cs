using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Subscriptions.GetAllDropboxesSubscriptions
{
  // Bu handler, subscriptions dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetAllDropboxesSubscriptionsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesSubscriptionsQueryRequest, TransactionResultPack<GetAllDropboxesSubscriptionsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllDropboxesSubscriptionsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllDropboxesSubscriptionsQueryResponse>> Handle(GetAllDropboxesSubscriptionsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 AuthCustomerId'yi önce request'ten al, yoksa token'dan al
        var authCustomerId = request.GetAuthCustomerIdAsGuid();

        // Eğer request'te yoksa, token'dan al
        if (!authCustomerId.HasValue)
        {
          authCustomerId = GetSafeCustomerId(request.AuthCustomerId);
        }

        // Hala yoksa hata döndür
        if (!authCustomerId.HasValue)
        {
          return ResultFactory.CreateErrorResult<GetAllDropboxesSubscriptionsQueryResponse>(
            request.AuthCustomerId,
            null,
            "Hata / Eksik Parametre",
            "Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
            "Customer ID parameter is required and not found in token."
          );
        }

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.subscriptions
          .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // Yetkilendirme filtreleri uygulanıyor
        var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

        // Sayfalama uygulanıyor
        var subscriptionss = await ApplyPagination(filteredQuery, request.Page, request.Size)
          .AsNoTracking()
          .ToListAsync(cancellationToken);

        if (subscriptionss == null || subscriptionss.Count == 0)
        {
          return ResultFactory.CreateErrorResult<GetAllDropboxesSubscriptionsQueryResponse>(
            request.AuthCustomerId,
            null,
            "Hata / subscriptions Bulunamadı",
            "Belirtilen müşteriye ait subscriptions bulunamadı.",
            $"No subscriptions found for customer ID: {authCustomerId}"
          );
        }

        // Dropdown için sadece gerekli alanları seçiyoruz
        var subscriptionsDetail = subscriptionss.Select(subscriptions => new subscriptionsDetailDto
        {
          Id = subscriptions.Id,
          AuthCustomerId = subscriptions.AuthCustomerId,
          AuthUserId = subscriptions.AuthUserId,
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
        }).ToList();

        // 🎯 Auth kullanıcı bilgilerini service ile al
        var authUserIds = subscriptionss.Select(subscriptions => subscriptions.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

        // Her subscriptions için kullanıcı bilgilerini ekle
        foreach (var subscriptions in subscriptionsDetail)
        {
          if (subscriptions.AuthUserId.HasValue && authUserDetails.ContainsKey(subscriptions.AuthUserId.Value))
          {
            var userDetail = authUserDetails[subscriptions.AuthUserId.Value];
            subscriptions.AuthUserName = userDetail.AuthUserName;
            subscriptions.AuthCustomerName = userDetail.AuthCustomerName;
          }
        }

        return ResultFactory.CreateSuccessResult<GetAllDropboxesSubscriptionsQueryResponse>(
          new GetAllDropboxesSubscriptionsQueryResponse
          {
            subscriptions = subscriptionsDetail
          },
          authCustomerId,
          null,
          "İşlem Başarılı",
          "subscriptions başarıyla getirildi.",
          $" subscriptions başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllDropboxesSubscriptionsQueryResponse>(
          request.AuthCustomerId,
          null,
          "Hata / İşlem Başarısız",
          "subscriptions getirilirken bir hata oluştu.",
          ex.Message
        );
      }
    }
  }
}
