using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllDropboxesBanners
{
	// Bu handler, banners dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBannersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBannersQueryRequest, TransactionResultPack<GetAllDropboxesBannersQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBannersQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBannersQueryResponse>> Handle(GetAllDropboxesBannersQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBannersQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.banners
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var bannerss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (bannerss == null || bannerss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBannersQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / banners Bulunamadı",
						"Belirtilen müşteriye ait banners bulunamadı.",
						$"No banners found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var bannersDetail = bannerss.Select(banners => new bannersDetailDto
				{
					Id = banners.Id,
					AuthCustomerId = banners.AuthCustomerId,
					AuthUserId = banners.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = bannerss.Select(banners => banners.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her banners için kullanıcı bilgilerini ekle
				foreach (var banners in bannersDetail)
				{
					if (banners.AuthUserId.HasValue && authUserDetails.ContainsKey(banners.AuthUserId.Value))
					{
						var userDetail = authUserDetails[banners.AuthUserId.Value];
						banners.AuthUserName = userDetail.AuthUserName;
						banners.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBannersQueryResponse>(
					new GetAllDropboxesBannersQueryResponse
					{
						banners = bannersDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"banners başarıyla getirildi.",
					$" banners başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBannersQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"banners getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
