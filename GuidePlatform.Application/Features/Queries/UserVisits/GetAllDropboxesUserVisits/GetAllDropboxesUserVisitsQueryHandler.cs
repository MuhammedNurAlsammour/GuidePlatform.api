using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetAllDropboxesUserVisits
{
	// Bu handler, userVisits dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesUserVisitsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesUserVisitsQueryRequest, TransactionResultPack<GetAllDropboxesUserVisitsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesUserVisitsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesUserVisitsQueryResponse>> Handle(GetAllDropboxesUserVisitsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesUserVisitsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.userVisits
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var userVisitss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (userVisitss == null || userVisitss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesUserVisitsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / userVisits Bulunamadı",
						"Belirtilen müşteriye ait userVisits bulunamadı.",
						$"No userVisits found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var userVisitsDetail = userVisitss.Select(userVisits => new userVisitsDetailDto
				{
					Id = userVisits.Id,
					AuthCustomerId = userVisits.AuthCustomerId,
					AuthUserId = userVisits.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = userVisitss.Select(userVisits => userVisits.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her userVisits için kullanıcı bilgilerini ekle
				foreach (var userVisits in userVisitsDetail)
				{
					if (userVisits.AuthUserId.HasValue && authUserDetails.ContainsKey(userVisits.AuthUserId.Value))
					{
						var userDetail = authUserDetails[userVisits.AuthUserId.Value];
						userVisits.AuthUserName = userDetail.AuthUserName;
						userVisits.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesUserVisitsQueryResponse>(
					new GetAllDropboxesUserVisitsQueryResponse
					{
						userVisits = userVisitsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"userVisits başarıyla getirildi.",
					$" userVisits başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesUserVisitsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"userVisits getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
