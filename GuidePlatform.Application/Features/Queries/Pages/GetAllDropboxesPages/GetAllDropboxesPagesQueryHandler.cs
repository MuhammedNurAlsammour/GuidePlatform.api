using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Pages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Pages.GetAllDropboxesPages
{
	// Bu handler, pages dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesPagesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesPagesQueryRequest, TransactionResultPack<GetAllDropboxesPagesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesPagesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesPagesQueryResponse>> Handle(GetAllDropboxesPagesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesPagesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.pages
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var pagess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (pagess == null || pagess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesPagesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / pages Bulunamadı",
						"Belirtilen müşteriye ait pages bulunamadı.",
						$"No pages found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var pagesDetail = pagess.Select(pages => new pagesDetailDto
				{
					Id = pages.Id,
					AuthCustomerId = pages.AuthCustomerId,
					AuthUserId = pages.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = pagess.Select(pages => pages.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her pages için kullanıcı bilgilerini ekle
				foreach (var pages in pagesDetail)
				{
					if (pages.AuthUserId.HasValue && authUserDetails.ContainsKey(pages.AuthUserId.Value))
					{
						var userDetail = authUserDetails[pages.AuthUserId.Value];
						pages.AuthUserName = userDetail.AuthUserName;
						pages.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesPagesQueryResponse>(
					new GetAllDropboxesPagesQueryResponse
					{
						pages = pagesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"pages başarıyla getirildi.",
					$" pages başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesPagesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"pages getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
