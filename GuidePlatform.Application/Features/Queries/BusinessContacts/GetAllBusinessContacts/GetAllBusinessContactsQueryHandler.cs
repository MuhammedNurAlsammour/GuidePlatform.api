using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllBusinessContacts
{
  public class GetAllBusinessContactsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBusinessContactsQueryRequest, TransactionResultPack<GetAllBusinessContactsQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllBusinessContactsQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllBusinessContactsQueryResponse>> Handle(GetAllBusinessContactsQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.businessContacts
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyBusinessContactsFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var businessContactss = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = businessContactss.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = businessContactss.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = businessContactss.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var businessContactsDetails = new List<BusinessContactsDTO>();  // 🎯 businessContactsDTO listesi oluştur

        foreach (var businessContacts in businessContactss)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (businessContacts.AuthUserId.HasValue && allUserDetails.ContainsKey(businessContacts.AuthUserId.Value))
          {
            var userDetail = allUserDetails[businessContacts.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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
            CreateUserId = businessContacts.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = businessContacts.UpdateUserId,
            UpdateUserName = updateUserName,
            RowCreatedDate = businessContacts.RowCreatedDate,
            RowUpdatedDate = businessContacts.RowUpdatedDate,
            RowIsActive = businessContacts.RowIsActive,
            RowIsDeleted = businessContacts.RowIsDeleted
          };

          businessContactsDetails.Add(businessContactsDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllBusinessContactsQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllBusinessContactsQueryResponse
            {
              TotalCount = totalCount,
              businessContacts = businessContactsDetails  // 🎯 businessContactsDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "businessContacts başarıyla getirildi.",
            $"businessContactss.Count businessContacts başarıyla getirildi."  // 🎯 businessContacts sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllBusinessContactsQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "businessContacts getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// İşletme iletişim filtrelerini uygular - Applies business contacts filters
    /// </summary>
    private IQueryable<BusinessContactsViewModel> ApplyBusinessContactsFilters(
        IQueryable<BusinessContactsViewModel> query,
        GetAllBusinessContactsQueryRequest request)
    {
      // 🏢 İşletme bilgileri filtreleri - Business information filters
      if (request.BusinessId.HasValue)
        query = query.Where(x => x.BusinessId == request.BusinessId.Value);

      // 📞 İletişim bilgileri filtreleri - Contact information filters
      if (!string.IsNullOrEmpty(request.ContactType))
        query = query.Where(x => x.ContactType != null && x.ContactType.Contains(request.ContactType));

      if (!string.IsNullOrEmpty(request.ContactValue))
        query = query.Where(x => x.ContactValue != null && x.ContactValue.Contains(request.ContactValue));

      if (request.IsPrimary.HasValue)
        query = query.Where(x => x.IsPrimary == request.IsPrimary.Value);

      return query;
    }
  }
}
