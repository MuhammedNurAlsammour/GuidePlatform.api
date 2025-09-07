using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Payments;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllPayments
{
  public class GetAllPaymentsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllPaymentsQueryRequest, TransactionResultPack<GetAllPaymentsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllPaymentsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllPaymentsQueryResponse>> Handle(GetAllPaymentsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.payments
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyPaymentsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var paymentss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = paymentss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = paymentss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = paymentss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var paymentsDetails = new List<PaymentsDTO>();  // 🎯 paymentsDTO listesi oluştur

        foreach (var payments in paymentss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (payments.AuthUserId.HasValue && allUserDetails.ContainsKey(payments.AuthUserId.Value))
          {
            var userDetail = allUserDetails[payments.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var paymentsDetail = new PaymentsDTO
          {
            Id = payments.Id,
            AuthUserId = payments.AuthUserId,
            AuthCustomerId = payments.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = payments.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = payments.UpdateUserId,
            UpdateUserName = updateUserName,
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

          paymentsDetails.Add(paymentsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllPaymentsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllPaymentsQueryResponse
            {
              TotalCount = totalCount,
              payments = paymentsDetails  // 🎯 paymentsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "payments başarıyla getirildi.",
            $"paymentss.Count payments başarıyla getirildi."  // 🎯 payments sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllPaymentsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "payments getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Payments filtrelerini uygular - Applies Payments filters
    /// </summary>
    private IQueryable<PaymentsViewModel> ApplyPaymentsFilters(
        IQueryable<PaymentsViewModel> query,
        GetAllPaymentsQueryRequest request)
    {
      // 🔍 Abonelik kimliği filtresi - Subscription ID filter
      if (request.SubscriptionId.HasValue)
      {
        query = query.Where(x => x.SubscriptionId == request.SubscriptionId.Value);
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

      // 🔍 Para birimi filtresi - Currency filter
      if (!string.IsNullOrWhiteSpace(request.Currency))
      {
        query = query.Where(x => x.Currency == request.Currency.Trim());
      }

      // 🔍 Ödeme yöntemi filtresi - Payment method filter
      if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
      {
        query = query.Where(x => x.PaymentMethod != null && x.PaymentMethod.Contains(request.PaymentMethod.Trim()));
      }

      // 🔍 İşlem kimliği filtresi - Transaction ID filter
      if (!string.IsNullOrWhiteSpace(request.TransactionId))
      {
        query = query.Where(x => x.TransactionId != null && x.TransactionId.Contains(request.TransactionId.Trim()));
      }

      // 🔍 Ödeme tarihi aralığı filtresi - Payment date range filter
      if (request.PaymentDateFrom.HasValue)
      {
        query = query.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value >= request.PaymentDateFrom.Value);
      }

      if (request.PaymentDateTo.HasValue)
      {
        query = query.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value <= request.PaymentDateTo.Value);
      }

      // 🔍 Durum filtresi - Status filter
      if (!string.IsNullOrWhiteSpace(request.Status))
      {
        query = query.Where(x => x.Status == request.Status.Trim());
      }

      // 🔍 Notlar filtresi - Notes filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.Notes))
      {
        query = query.Where(x => x.Notes != null && x.Notes.Contains(request.Notes.Trim()));
      }

      return query;
    }
  }
}
