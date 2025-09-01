using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.categories.GetAllDropboxesCategories
{
	// Bu handler, categories dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesCategoriesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesCategoriesQueryRequest, TransactionResultPack<GetAllDropboxesCategoriesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesCategoriesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesCategoriesQueryResponse>> Handle(GetAllDropboxesCategoriesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesCategoriesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.categories
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var categoriess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (categoriess == null || categoriess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesCategoriesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / categories Bulunamadı",
						"Belirtilen müşteriye ait categories bulunamadı.",
						$"No categories found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var categoriesDetail = categoriess.Select(categories => new categoriesDetailDto
				{
					Id = categories.Id,
					AuthCustomerId = categories.AuthCustomerId,
					AuthUserId = categories.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = categoriess.Select(categories => categories.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her categories için kullanıcı bilgilerini ekle
				foreach (var categories in categoriesDetail)
				{
					if (categories.AuthUserId.HasValue && authUserDetails.ContainsKey(categories.AuthUserId.Value))
					{
						var userDetail = authUserDetails[categories.AuthUserId.Value];
						categories.AuthUserName = userDetail.AuthUserName;
						categories.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesCategoriesQueryResponse>(
					new GetAllDropboxesCategoriesQueryResponse
					{
						categories = categoriesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"categories başarıyla getirildi.",
					$" categories başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesCategoriesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"categories getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
