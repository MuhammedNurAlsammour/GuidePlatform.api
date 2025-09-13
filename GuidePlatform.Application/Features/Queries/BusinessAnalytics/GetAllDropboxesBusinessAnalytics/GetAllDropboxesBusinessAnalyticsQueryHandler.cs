using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllDropboxesBusinessAnalytics
{
	// Bu handler, businessAnalytics dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessAnalyticsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessAnalyticsQueryRequest, TransactionResultPack<GetAllDropboxesBusinessAnalyticsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessAnalyticsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessAnalyticsQueryResponse>> Handle(GetAllDropboxesBusinessAnalyticsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessAnalyticsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessAnalytics
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessAnalyticss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessAnalyticss == null || businessAnalyticss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessAnalyticsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessAnalytics Bulunamadı",
						"Belirtilen müşteriye ait businessAnalytics bulunamadı.",
						$"No businessAnalytics found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessAnalyticsDetail = businessAnalyticss.Select(businessAnalytics => new businessAnalyticsDetailDto
				{
					Id = businessAnalytics.Id,
					AuthCustomerId = businessAnalytics.AuthCustomerId,
					AuthUserId = businessAnalytics.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessAnalyticss.Select(businessAnalytics => businessAnalytics.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessAnalytics için kullanıcı bilgilerini ekle
				foreach (var businessAnalytics in businessAnalyticsDetail)
				{
					if (businessAnalytics.AuthUserId.HasValue && authUserDetails.ContainsKey(businessAnalytics.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessAnalytics.AuthUserId.Value];
						businessAnalytics.AuthUserName = userDetail.AuthUserName;
						businessAnalytics.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessAnalyticsQueryResponse>(
					new GetAllDropboxesBusinessAnalyticsQueryResponse
					{
						businessAnalytics = businessAnalyticsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessAnalytics başarıyla getirildi.",
					$" businessAnalytics başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessAnalyticsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessAnalytics getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
