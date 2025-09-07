using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Notifications;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllDropboxesNotifications
{
	// Bu handler, notifications dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesNotificationsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesNotificationsQueryRequest, TransactionResultPack<GetAllDropboxesNotificationsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesNotificationsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesNotificationsQueryResponse>> Handle(GetAllDropboxesNotificationsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.notifications
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var notificationss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (notificationss == null || notificationss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / notifications Bulunamadı",
						"Belirtilen müşteriye ait notifications bulunamadı.",
						$"No notifications found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var notificationsDetail = notificationss.Select(notifications => new notificationsDetailDto
				{
					Id = notifications.Id,
					AuthCustomerId = notifications.AuthCustomerId,
					AuthUserId = notifications.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = notificationss.Select(notifications => notifications.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her notifications için kullanıcı bilgilerini ekle
				foreach (var notifications in notificationsDetail)
				{
					if (notifications.AuthUserId.HasValue && authUserDetails.ContainsKey(notifications.AuthUserId.Value))
					{
						var userDetail = authUserDetails[notifications.AuthUserId.Value];
						notifications.AuthUserName = userDetail.AuthUserName;
						notifications.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesNotificationsQueryResponse>(
					new GetAllDropboxesNotificationsQueryResponse
					{
						notifications = notificationsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"notifications başarıyla getirildi.",
					$" notifications başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesNotificationsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"notifications getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
