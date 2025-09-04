using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllBusinessWorkingHours
{
  public class GetAllBusinessWorkingHoursQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessWorkingHoursQueryRequest, TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessWorkingHoursQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>> Handle(GetAllBusinessWorkingHoursQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessWorkingHours
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // BusinessId filtresi ekle (eğer verilmişse)
        if (!string.IsNullOrWhiteSpace(request.BusinessId))
        {
          var businessId = Guid.Parse(request.BusinessId);
          baseQuery = baseQuery.Where(x => x.BusinessId == businessId);
        }

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessWorkingHourss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessWorkingHourss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessWorkingHourss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessWorkingHourss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessWorkingHoursDetails = new List<BusinessWorkingHoursDTO>();  // 🎯 businessWorkingHoursDTO listesi oluştur

        foreach (var businessWorkingHours in businessWorkingHourss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businessWorkingHours.AuthUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businessWorkingHours.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
          }

          // 🎯 Create/Update kullanıcı bilgilerini al
          string? createUserName = null;
          string? updateUserName = null;

          if (businessWorkingHours.CreateUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.CreateUserId.Value))
          {
            var createUserDetail = allUserDetails[businessWorkingHours.CreateUserId.Value];
            createUserName = createUserDetail.AuthUserName;
          }

          if (businessWorkingHours.UpdateUserId.HasValue && allUserDetails.ContainsKey(businessWorkingHours.UpdateUserId.Value))
          {
            var updateUserDetail = allUserDetails[businessWorkingHours.UpdateUserId.Value];
            updateUserName = updateUserDetail.AuthUserName;
          }

          var businessWorkingHoursDetail = new BusinessWorkingHoursDTO
          {
            Id = businessWorkingHours.Id,
            BusinessId = businessWorkingHours.BusinessId,
            DayOfWeek = businessWorkingHours.DayOfWeek,
            OpenTime = businessWorkingHours.OpenTime,
            CloseTime = businessWorkingHours.CloseTime,
            IsClosed = businessWorkingHours.IsClosed,
            Icon = businessWorkingHours.Icon,
            AuthUserId = businessWorkingHours.AuthUserId,
            AuthCustomerId = businessWorkingHours.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = businessWorkingHours.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessWorkingHours.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessWorkingHours.RowCreatedDate,
            RowUpdatedDate = businessWorkingHours.RowUpdatedDate,
            RowIsActive = businessWorkingHours.RowIsActive,
            RowIsDeleted = businessWorkingHours.RowIsDeleted
          };

          businessWorkingHoursDetails.Add(businessWorkingHoursDetail);
        }

        // Mesaj oluştur
        string messageDetail;
        if (!string.IsNullOrWhiteSpace(request.BusinessId))
        {
          messageDetail = $"BusinessId: {request.BusinessId} için {businessWorkingHoursDetails.Count} adet çalışma saati başarıyla getirildi.";
        }
        else
        {
          messageDetail = $"{businessWorkingHoursDetails.Count} adet çalışma saati başarıyla getirildi.";
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessWorkingHoursQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessWorkingHoursQueryResponse
            {
              TotalCount = totalCount,
              businessWorkingHours = businessWorkingHoursDetails  // 🎯 businessWorkingHoursDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessWorkingHours başarıyla getirildi.",
            messageDetail
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessWorkingHoursQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessWorkingHours getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}
