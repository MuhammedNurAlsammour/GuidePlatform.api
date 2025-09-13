using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Files;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllFiles
{
  public class GetAllFilesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllFilesQueryRequest, TransactionResultPack<GetAllFilesQueryResponse>>
  {
    private readonly IApplicationDbContext _context;

    public GetAllFilesQueryHandler(
      IApplicationDbContext context,
      IAuthUserDetailService authUserService,
      ICurrentUserService currentUserService) : base(currentUserService, authUserService)
    {
      _context = context;
    }

    public async Task<TransactionResultPack<GetAllFilesQueryResponse>> Handle(GetAllFilesQueryRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 🎯 Token'dan güvenli bir şekilde Auth bilgilerini al
        var authUserId = GetSafeUserId(request.AuthUserId);
        var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

        // Base query oluştur
        var baseQuery = _context.files
        .Where(x => x.RowIsActive && !x.RowIsDeleted);

        // 🎯 Filtreleme uygula - Apply filtering
        baseQuery = ApplyFilesFilters(baseQuery, request);

        // 🎯 Toplam sayıyı hesapla (filtreleme sonrası) - Düzeltilmiş filtreleme
        var totalCountQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var totalCount = await totalCountQuery
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // 🎯 Verileri getir (filtreleme + sayfalama) - Düzeltilmiş filtreleme
        var filteredQuery = ApplyAuthFilters(baseQuery, authUserId, authCustomerId);
        var filess = await ApplyPagination(filteredQuery, request.GetValidatedPage(), request.GetValidatedSize())
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        // 🎯 Tüm kullanıcı bilgilerini toplu olarak al (performans için)
        var allUserIds = new List<Guid>();

        // Auth kullanıcı bilgileri
        var authUserIds = filess.Where(b => b.AuthUserId.HasValue).Select(b => b.AuthUserId!.Value).ToList();
        allUserIds.AddRange(authUserIds);

        // Create kullanıcı bilgileri
        var createUserIds = filess.Where(b => b.CreateUserId.HasValue).Select(b => b.CreateUserId!.Value).ToList();
        allUserIds.AddRange(createUserIds);

        // Update kullanıcı bilgileri
        var updateUserIds = filess.Where(b => b.UpdateUserId.HasValue).Select(b => b.UpdateUserId!.Value).ToList();
        allUserIds.AddRange(updateUserIds);

        // Tek seferde tüm kullanıcı bilgilerini al
        var allUserDetails = await _authUserService.GetAuthUserDetailsAsync(allUserIds.Distinct().ToList(), cancellationToken);

        var filesDetails = new List<FilesDTO>();  // 🎯 filesDTO listesi oluştur

        foreach (var files in filess)
        {
          // 🎯 AuthUserService'den kullanıcı bilgilerini al
          string? authUserName = null;
          string? authCustomerName = null;

          if (files.AuthUserId.HasValue && allUserDetails.ContainsKey(files.AuthUserId.Value))
          {
            var userDetail = allUserDetails[files.AuthUserId.Value];
            authUserName = userDetail.AuthUserName;        // AspNetUsers.UserName
            authCustomerName = userDetail.AuthCustomerName; // Customers.Name
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

          var filesDetail = new FilesDTO
          {
            Id = files.Id,
            AuthUserId = files.AuthUserId,
            AuthCustomerId = files.AuthCustomerId,
            AuthUserName = authUserName,      // Service'den gelen
            AuthCustomerName = authCustomerName, // Service'den gelen
            CreateUserId = files.CreateUserId,
            CreateUserName = createUserName,
            UpdateUserId = files.UpdateUserId,
            UpdateUserName = updateUserName,
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

          filesDetails.Add(filesDetail);
        }

        return ResultFactory.CreateSuccessResult<GetAllFilesQueryResponse>(  // 🎯 TransactionResultPack oluştur
            new GetAllFilesQueryResponse
            {
              TotalCount = totalCount,
              files = filesDetails  // 🎯 filesDTO listesini döndür
            },
            null,
            null,
            "İşlem Başarılı",
            "files başarıyla getirildi.",
            $"filess.Count files başarıyla getirildi."  // 🎯 files sayısını göster
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<GetAllFilesQueryResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "files getirilirken bir hata oluştu.",
            ex.Message
        );
      }
    }

    /// <summary>
    /// Kullanıcı Files filtrelerini uygular - Applies Files filters
    /// </summary>
    private IQueryable<FilesViewModel> ApplyFilesFilters(
        IQueryable<FilesViewModel> query,
        GetAllFilesQueryRequest request)
    {
      // 🔍 Dosya türü filtresi - File type filter
      if (request.FileType.HasValue)
      {
        query = query.Where(x => x.FileType == request.FileType.Value);
      }

      // 🔍 Genel erişim filtresi - Public access filter
      if (request.IsPublic.HasValue)
      {
        query = query.Where(x => x.IsPublic == request.IsPublic.Value);
      }

      // 🔍 Dosya adı filtresi - File name filter (içerik arama)
      if (!string.IsNullOrWhiteSpace(request.FileName))
      {
        query = query.Where(x => x.FileName.Contains(request.FileName.Trim()));
      }

      // 🔍 MIME türü filtresi - MIME type filter
      if (!string.IsNullOrWhiteSpace(request.MimeType))
      {
        query = query.Where(x => x.MimeType != null && x.MimeType.Contains(request.MimeType.Trim()));
      }

      return query;
    }
  }
}
