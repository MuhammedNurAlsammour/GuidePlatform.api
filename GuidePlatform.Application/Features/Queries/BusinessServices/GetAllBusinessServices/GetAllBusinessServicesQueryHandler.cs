using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllBusinessServices
{
  public class GetAllBusinessServicesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessServicesQueryRequest, TransactionResultPack<GetAllBusinessServicesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessServicesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessServicesQueryResponse>> Handle(GetAllBusinessServicesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessServices
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessServicess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);
  
        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessServicess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessServicess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessServicess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessServicesDetails = new List<BusinessServicesDTO>();  // 🎯 businessServicesDTO listesi oluştur

        foreach (var businessServices in businessServicess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businessServices.AuthUserId.HasValue && allUserDetails.ContainsKey(businessServices.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businessServices.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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
            CreateUserId = businessServices.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessServices.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessServices.RowCreatedDate,
            RowUpdatedDate = businessServices.RowUpdatedDate,
            RowIsActive = businessServices.RowIsActive,
            RowIsDeleted = businessServices.RowIsDeleted
          };

          businessServicesDetails.Add(businessServicesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessServicesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessServicesQueryResponse
            {
              TotalCount = totalCount,
              businessServices = businessServicesDetails  // 🎯 businessServicesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessServices başarıyla getirildi.",
            $"businessServicess.Count businessServices başarıyla getirildi."  // 🎯 businessServices sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessServicesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessServices getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}
