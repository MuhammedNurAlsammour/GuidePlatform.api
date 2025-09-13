using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Announcements;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllDropboxesAnnouncements
{
	// Bu handler, announcements dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesAnnouncementsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesAnnouncementsQueryRequest, TransactionResultPack<GetAllDropboxesAnnouncementsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesAnnouncementsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesAnnouncementsQueryResponse>> Handle(GetAllDropboxesAnnouncementsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesAnnouncementsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.announcements
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var announcementss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (announcementss == null || announcementss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesAnnouncementsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / announcements Bulunamadı",
						"Belirtilen müşteriye ait announcements bulunamadı.",
						$"No announcements found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var announcementsDetail = announcementss.Select(announcements => new announcementsDetailDto
				{
					Id = announcements.Id,
					AuthCustomerId = announcements.AuthCustomerId,
					AuthUserId = announcements.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = announcementss.Select(announcements => announcements.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her announcements için kullanıcı bilgilerini ekle
				foreach (var announcements in announcementsDetail)
				{
					if (announcements.AuthUserId.HasValue && authUserDetails.ContainsKey(announcements.AuthUserId.Value))
					{
						var userDetail = authUserDetails[announcements.AuthUserId.Value];
						announcements.AuthUserName = userDetail.AuthUserName;
						announcements.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesAnnouncementsQueryResponse>(
					new GetAllDropboxesAnnouncementsQueryResponse
					{
						announcements = announcementsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"announcements başarıyla getirildi.",
					$" announcements başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesAnnouncementsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"announcements getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
