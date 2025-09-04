using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllDropboxesBusinessWorkingHours
{
	// Bu handler, businessWorkingHours dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessWorkingHoursQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessWorkingHoursQueryRequest, TransactionResultPack<GetAllDropboxesBusinessWorkingHoursQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessWorkingHoursQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessWorkingHoursQueryResponse>> Handle(GetAllDropboxesBusinessWorkingHoursQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessWorkingHoursQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessWorkingHours
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessWorkingHourss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessWorkingHourss == null || businessWorkingHourss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessWorkingHoursQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessWorkingHours Bulunamadı",
						"Belirtilen müşteriye ait businessWorkingHours bulunamadı.",
						$"No businessWorkingHours found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessWorkingHoursDetail = businessWorkingHourss.Select(businessWorkingHours => new businessWorkingHoursDetailDto
				{
					Id = businessWorkingHours.Id,
					AuthCustomerId = businessWorkingHours.AuthCustomerId,
					AuthUserId = businessWorkingHours.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessWorkingHourss.Select(businessWorkingHours => businessWorkingHours.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessWorkingHours için kullanıcı bilgilerini ekle
				foreach (var businessWorkingHours in businessWorkingHoursDetail)
				{
					if (businessWorkingHours.AuthUserId.HasValue && authUserDetails.ContainsKey(businessWorkingHours.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessWorkingHours.AuthUserId.Value];
						businessWorkingHours.AuthUserName = userDetail.AuthUserName;
						businessWorkingHours.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessWorkingHoursQueryResponse>(
					new GetAllDropboxesBusinessWorkingHoursQueryResponse
					{
						businessWorkingHours = businessWorkingHoursDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessWorkingHours başarıyla getirildi.",
					$" businessWorkingHours başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessWorkingHoursQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessWorkingHours getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
