using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllDropboxesBusinessReviews
{
	// Bu handler, businessReviews dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessReviewsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessReviewsQueryRequest, TransactionResultPack<GetAllDropboxesBusinessReviewsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessReviewsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessReviewsQueryResponse>> Handle(GetAllDropboxesBusinessReviewsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessReviewsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessReviews
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessReviewss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessReviewss == null || businessReviewss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessReviewsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessReviews Bulunamadı",
						"Belirtilen müşteriye ait businessReviews bulunamadı.",
						$"No businessReviews found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessReviewsDetail = businessReviewss.Select(businessReviews => new businessReviewsDetailDto
				{
					Id = businessReviews.Id,
					AuthCustomerId = businessReviews.AuthCustomerId,
					AuthUserId = businessReviews.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessReviewss.Select(businessReviews => businessReviews.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessReviews için kullanıcı bilgilerini ekle
				foreach (var businessReviews in businessReviewsDetail)
				{
					if (businessReviews.AuthUserId.HasValue && authUserDetails.ContainsKey(businessReviews.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessReviews.AuthUserId.Value];
						businessReviews.AuthUserName = userDetail.AuthUserName;
						businessReviews.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessReviewsQueryResponse>(
					new GetAllDropboxesBusinessReviewsQueryResponse
					{
						businessReviews = businessReviewsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessReviews başarıyla getirildi.",
					$" businessReviews başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessReviewsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessReviews getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
