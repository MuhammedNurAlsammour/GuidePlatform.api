using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.SearchLogs.GetAllDropboxesSearchLogs
{
	// Bu handler, searchLogs dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesSearchLogsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesSearchLogsQueryRequest, TransactionResultPack<GetAllDropboxesSearchLogsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesSearchLogsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesSearchLogsQueryResponse>> Handle(GetAllDropboxesSearchLogsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesSearchLogsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.searchLogs
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var searchLogss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (searchLogss == null || searchLogss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesSearchLogsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / searchLogs Bulunamadı",
						"Belirtilen müşteriye ait searchLogs bulunamadı.",
						$"No searchLogs found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var searchLogsDetail = searchLogss.Select(searchLogs => new searchLogsDetailDto
				{
					Id = searchLogs.Id,
					AuthCustomerId = searchLogs.AuthCustomerId,
					AuthUserId = searchLogs.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = searchLogss.Select(searchLogs => searchLogs.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her searchLogs için kullanıcı bilgilerini ekle
				foreach (var searchLogs in searchLogsDetail)
				{
					if (searchLogs.AuthUserId.HasValue && authUserDetails.ContainsKey(searchLogs.AuthUserId.Value))
					{
						var userDetail = authUserDetails[searchLogs.AuthUserId.Value];
						searchLogs.AuthUserName = userDetail.AuthUserName;
						searchLogs.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesSearchLogsQueryResponse>(
					new GetAllDropboxesSearchLogsQueryResponse
					{
						searchLogs = searchLogsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"searchLogs başarıyla getirildi.",
					$" searchLogs başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesSearchLogsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"searchLogs getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
