using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllDropboxesUserFavorites
{
	// Bu handler, userFavorites dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesUserFavoritesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesUserFavoritesQueryRequest, TransactionResultPack<GetAllDropboxesUserFavoritesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesUserFavoritesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesUserFavoritesQueryResponse>> Handle(GetAllDropboxesUserFavoritesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesUserFavoritesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.userFavorites
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var userFavoritess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (userFavoritess == null || userFavoritess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesUserFavoritesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / userFavorites Bulunamadı",
						"Belirtilen müşteriye ait userFavorites bulunamadı.",
						$"No userFavorites found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var userFavoritesDetail = userFavoritess.Select(userFavorites => new userFavoritesDetailDto
				{
					Id = userFavorites.Id,
					AuthCustomerId = userFavorites.AuthCustomerId,
					AuthUserId = userFavorites.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = userFavoritess.Select(userFavorites => userFavorites.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her userFavorites için kullanıcı bilgilerini ekle
				foreach (var userFavorites in userFavoritesDetail)
				{
					if (userFavorites.AuthUserId.HasValue && authUserDetails.ContainsKey(userFavorites.AuthUserId.Value))
					{
						var userDetail = authUserDetails[userFavorites.AuthUserId.Value];
						userFavorites.AuthUserName = userDetail.AuthUserName;
						userFavorites.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesUserFavoritesQueryResponse>(
					new GetAllDropboxesUserFavoritesQueryResponse
					{
						userFavorites = userFavoritesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"userFavorites başarıyla getirildi.",
					$" userFavorites başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesUserFavoritesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"userFavorites getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
