using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Files;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Files.GetFilesById
{
  // Bu handler, bir files ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
  public class GetFilesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetFilesByIdQueryRequest, TransactionResultPack<GetFilesByIdQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetFilesByIdQueryHandler(
        IApplicationDbContext context,
        IAuthUserDetailService authUserService,
        ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetFilesByIdQueryResponse>> Handle(GetFilesByIdQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 ID doğrulama - ID parametresi kontrolü
        if (string.IsNullOrEmpty(request.Id))
        {
          // Eksik parametre hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetFilesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Eksik Parametre",
              "files ID'si belirtilmedi.",
              "files ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
          );
        }

        var filesId = request.GetIdAsGuid();
        if (!filesId.HasValue)
        {
          // Geçersiz ID formatı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetFilesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / Geçersiz ID",
              "Geçersiz files ID formatı.",
              $"Geçersiz files ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
          );
        }

        // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Temel sorgu oluşturuluyor
        var baseQuery = _context.files
            .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .Where(x => x.Id == filesId.Value)
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Yetkilendirme filtreleri uygulanıyor ve files çekiliyor
        var files = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
            .Where(x => x.Id == filesId.Value)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (files == null)
        {
          // files bulunamadı hatası döndürülüyor
          return ResultFactory.CreateErrorResult<GetFilesByIdQueryResponse>(
              request.Id,
              null,
              "Hata / files Bulunamadı",
              "Belirtilen ID'ye sahip files bulunamadı.",
              $"ID '{request.Id}' ile eşleşen files bulunamadı."
          );
        }

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        if (files.AuthUserId.HasValue)
          allUserIds.Add(files.AuthUserId.Value);

        // Create kullanıcı bilgileri
        if (files.CreateUserId.HasValue)
          allUserIds.Add(files.CreateUserId.Value);

        // Update kullanıcı bilgileri
        if (files.UpdateUserId.HasValue)
          allUserIds.Add(files.UpdateUserId.Value);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        // 🎯 Auth kullanıcı bilgilerini al
        string? authUserName = null;
        string? authCustomerName = null;

        if (files.AuthUserId.HasValue && allUserDetails.ContainsKey(files.AuthUserId.Value))
        {
          var userDetail = allUserDetails[files.AuthUserId.Value];
          authUserName = userDetail.AuthUserName;
          authCustomerName = userDetail.AuthCustomerName;
        }

        // 🎯 Create/Update kullanıcı bilgilerini al
        string? createUserName = null;
        string? updateUserName = null;

        if (files.CreateUserId.HasValue && allUserDetails.ContainsKey(files.CreateUserId.Value))
        {
          var createUserDetail = allUserDetails[files.CreateUserId.Value];
          createUserName = createUserDetail.AuthUserName;
        }

        if (files.UpdateUserId.HasValue && allUserDetails.ContainsKey(files.UpdateUserId.Value))
        {
          var updateUserDetail = allUserDetails[files.UpdateUserId.Value];
          updateUserName = updateUserDetail.AuthUserName;
        }

        // files detay DTO'su oluşturuluyor
        var filesDetail = new FilesDTO
        {
          Id = files.Id,
          AuthUserId = files.AuthUserId,
          AuthCustomerId = files.AuthCustomerId,
          AuthUserName = authUserName,      // Service'den gelen
          AuthCustomerName = authCustomerName, // Service'den gelen
          CreateUserName = createUserName,  // Service'den gelen
          UpdateUserName = updateUserName,  // Service'den gelen
          CreateUserId = files.CreateUserId,
          UpdateUserId = files.UpdateUserId,
          RowCreatedDate = files.RowCreatedDate,
          RowUpdatedDate = files.RowUpdatedDate,
          RowIsActive = files.RowIsActive,
          RowIsDeleted = files.RowIsDeleted,
          // Dosya özel alanları - File specific fields
          FileName = files.FileName,
          FilePath = files.FilePath,
          FileSize = files.FileSize,
          MimeType = files.MimeType,
          FileType = files.FileType,
          IsPublic = files.IsPublic,
          Icon = files.Icon
        };

        // Başarılı işlem sonucu döndürülüyor
        return ResultFactory.CreateSuccessResult<GetFilesByIdQueryResponse>(
            new GetFilesByIdQueryResponse
            {
              files = filesDetail,
              TotalCount = totalCount
            },
            request.Id,
            null,
            "İşlem Başarılı",
            "files başarıyla getirildi.",
            $"files Id: {files.Id} başarıyla getirildi."
        );
      }
      catch (Exception ex)
      {
        // Hata durumunda hata sonucu döndürülüyor
        return ResultFactory.CreateErrorResult<GetFilesByIdQueryResponse>(
            request.Id,
            null,
            "Hata / İşlem Başarısız",
            "files getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }
  }
}

