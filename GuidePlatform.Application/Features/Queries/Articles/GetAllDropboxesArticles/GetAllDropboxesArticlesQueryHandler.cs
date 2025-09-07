using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Articles;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllDropboxesArticles
{
	// Bu handler, articles dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesArticlesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesArticlesQueryRequest, TransactionResultPack<GetAllDropboxesArticlesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesArticlesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesArticlesQueryResponse>> Handle(GetAllDropboxesArticlesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesArticlesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.articles
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var articless = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (articless == null || articless.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesArticlesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / articles Bulunamadı",
						"Belirtilen müşteriye ait articles bulunamadı.",
						$"No articles found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var articlesDetail = articless.Select(articles => new articlesDetailDto
				{
					Id = articles.Id,
					AuthCustomerId = articles.AuthCustomerId,
					AuthUserId = articles.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = articless.Select(articles => articles.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her articles için kullanıcı bilgilerini ekle
				foreach (var articles in articlesDetail)
				{
					if (articles.AuthUserId.HasValue && authUserDetails.ContainsKey(articles.AuthUserId.Value))
					{
						var userDetail = authUserDetails[articles.AuthUserId.Value];
						articles.AuthUserName = userDetail.AuthUserName;
						articles.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesArticlesQueryResponse>(
					new GetAllDropboxesArticlesQueryResponse
					{
						articles = articlesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"articles başarıyla getirildi.",
					$" articles başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesArticlesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"articles getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
