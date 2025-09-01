using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.categories.GetCategoriesById
{
    // Bu handler, bir categories ID'ye göre getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
    public class GetCategoriesByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetCategoriesByIdQueryRequest, TransactionResultPack<GetCategoriesByIdQueryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetCategoriesByIdQueryHandler(
            IApplicationDbContext context,
            IAuthUserDetailService authUserService,
            ICurrentUserService currentUserService) : base(currentUserService, authUserService)
        {
            _context = context;
        }

        public async Task<TransactionResultPack<GetCategoriesByIdQueryResponse>> Handle(GetCategoriesByIdQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // 🎯 ID doğrulama - ID parametresi kontrolü
                if (string.IsNullOrEmpty(request.Id))
                {
                    // Eksik parametre hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetCategoriesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Eksik Parametre",
                        "categories ID'si belirtilmedi.",
                        "categories ID parametresi zorunludur. Lütfen 'Id' parametresini sorguya ekleyin."
                    );
                }

                var categoriesId = request.GetIdAsGuid();
                if (!categoriesId.HasValue)
                {
                    // Geçersiz ID formatı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetCategoriesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / Geçersiz ID",
                        "Geçersiz categories ID formatı.",
                        $"Geçersiz categories ID formatı: '{request.Id}'. Lütfen geçerli bir GUID girin."
                    );
                }

                // Kullanıcı ve müşteri kimliklerini güvenli şekilde al
                var authUserId = GetSafeUserId(request.AuthUserId);
                var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

                // Temel sorgu oluşturuluyor
                var baseQuery = _context.categories
                    .Where(x => x.RowIsActive && !x.RowIsDeleted);

                // Yetkilendirme filtreleri uygulanıyor ve categories çekiliyor
                var categories = await ApplyAuthFilters(baseQuery, authUserId, authCustomerId)
                    .Where(x => x.Id == categoriesId.Value) 
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (categories == null)
                {
                    // categories bulunamadı hatası döndürülüyor
                    return ResultFactory.CreateErrorResult<GetCategoriesByIdQueryResponse>(
                        request.Id,
                        null,
                        "Hata / categories Bulunamadı",
                        "Belirtilen ID'ye sahip categories bulunamadı.",
                        $"ID '{request.Id}' ile eşleşen categories bulunamadı."
                    );
                }

                // Auth kullanıcı bilgileri çekiliyor
                string? authUserName = null;
                string? authCustomerName = null;

                if (categories.AuthUserId.HasValue)
                {
                    var userDetail = await GetAuthUserDetailAsync(categories.AuthUserId.Value, cancellationToken);
                    if (userDetail != null)
                    {
                        authUserName = userDetail.AuthUserName;
                        authCustomerName = userDetail.AuthCustomerName;
                    }
                }

                // Parent kategori bilgilerini al
                string? parentName = null;
                if (categories.ParentId.HasValue)
                {
                    var parentCategory = await _context.categories
                        .Where(x => x.Id == categories.ParentId.Value && x.RowIsActive && !x.RowIsDeleted)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
                    
                    if (parentCategory != null)
                    {
                        parentName = parentCategory.Name;
                    }
                }

                // Alt kategori sayısını hesapla
                var childrenCount = await _context.categories
                    .Where(x => x.ParentId == categories.Id && x.RowIsActive && !x.RowIsDeleted)
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                // categories detay DTO'su oluşturuluyor
                var categoriesDetail = new GetCategoriesByIdDetailDto
                {
                    Id = categories.Id,
                    Name = categories.Name,
                    Description = categories.Description,
                    ParentId = categories.ParentId,
                    ParentName = parentName,
                    Icon = categories.Icon,
                    SortOrder = categories.SortOrder,
                    ChildrenCount = childrenCount,
                    FullPath = await GetCategoryFullPath(categories, cancellationToken),
                    AuthUserId = categories.AuthUserId,
                    AuthCustomerId = categories.AuthCustomerId,
                    AuthUserName = authUserName,      // Service'den gelen
                    AuthCustomerName = authCustomerName, // Service'den gelen
                    CreateUserId = categories.CreateUserId,
                    UpdateUserId = categories.UpdateUserId,
                    RowCreatedDate = categories.RowCreatedDate,
                    RowUpdatedDate = categories.RowUpdatedDate,
                    RowIsActive = categories.RowIsActive,
                    RowIsDeleted = categories.RowIsDeleted
                };

                // Başarılı işlem sonucu döndürülüyor
                return ResultFactory.CreateSuccessResult<GetCategoriesByIdQueryResponse>(
                    new GetCategoriesByIdQueryResponse
                    {
                        categories = categoriesDetail
                    },
                    request.Id,
                    null,
                    "İşlem Başarılı",
                    "categories başarıyla getirildi.",
                    $"categories Id: {categories.Id} başarıyla getirildi."
                );
            }
            catch (Exception ex)
            {
                // Hata durumunda hata sonucu döndürülüyor
                return ResultFactory.CreateErrorResult<GetCategoriesByIdQueryResponse>(
                    request.Id,
                    null,
                    "Hata / İşlem Başarısız",
                    "categories getirilirken bir hata oluştu.",
                    ex.Message
                );
            }
        }

        // 🎯 Kategori tam yolunu oluşturan yardımcı method
        private async Task<string> GetCategoryFullPath(Domain.Entities.CategoriesViewModel categories, CancellationToken cancellationToken)
        {
            var pathParts = new List<string> { categories.Name };
            var currentCategory = categories;

            // Parent kategorileri takip ederek tam yolu oluştur
            while (currentCategory.ParentId.HasValue)
            {
                var parent = await _context.categories
                    .Where(x => x.Id == currentCategory.ParentId.Value && x.RowIsActive && !x.RowIsDeleted)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (parent == null)
                    break;

                pathParts.Insert(0, parent.Name);
                currentCategory = parent;
            }

            return string.Join(" / ", pathParts);
        }
    }
}

