using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllDropboxesNotificationSettings
{
	// Bu handler, notificationSettings dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesNotificationSettingsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesNotificationSettingsQueryRequest, TransactionResultPack<GetAllDropboxesNotificationSettingsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesNotificationSettingsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesNotificationSettingsQueryResponse>> Handle(GetAllDropboxesNotificationSettingsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationSettingsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.notificationSettings
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var notificationSettingss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (notificationSettingss == null || notificationSettingss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationSettingsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / notificationSettings Bulunamadı",
						"Belirtilen müşteriye ait notificationSettings bulunamadı.",
						$"No notificationSettings found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var notificationSettingsDetail = notificationSettingss.Select(notificationSettings => new notificationSettingsDetailDto
				{
					Id = notificationSettings.Id,
					AuthCustomerId = notificationSettings.AuthCustomerId,
					AuthUserId = notificationSettings.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = notificationSettingss.Select(notificationSettings => notificationSettings.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her notificationSettings için kullanıcı bilgilerini ekle
				foreach (var notificationSettings in notificationSettingsDetail)
				{
					if (notificationSettings.AuthUserId.HasValue && authUserDetails.ContainsKey(notificationSettings.AuthUserId.Value))
					{
						var userDetail = authUserDetails[notificationSettings.AuthUserId.Value];
						notificationSettings.AuthUserName = userDetail.AuthUserName;
						notificationSettings.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesNotificationSettingsQueryResponse>(
					new GetAllDropboxesNotificationSettingsQueryResponse
					{
						notificationSettings = notificationSettingsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"notificationSettings başarıyla getirildi.",
					$" notificationSettings başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationSettingsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"notificationSettings getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
