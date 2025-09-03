using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetAllDropboxesBusinesses
{
	// Bu handler, Businesses dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessesQueryRequest, TransactionResultPack<GetAllDropboxesBusinessesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessesQueryResponse>> Handle(GetAllDropboxesBusinessesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businesses
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessess == null || businessess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Businesses Bulunamadı",
						"Belirtilen müşteriye ait Businesses bulunamadı.",
						$"No Businesses found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessesDetail = businessess.Select(businesses => new businessesDetailDto
				{
					Id = businesses.Id,
					AuthCustomerId = businesses.AuthCustomerId,
					AuthUserId = businesses.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessess.Select(businesses => businesses.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businesses için kullanıcı bilgilerini ekle
				foreach (var businesses in businessesDetail)
				{
					if (businesses.AuthUserId.HasValue && authUserDetails.ContainsKey(businesses.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businesses.AuthUserId.Value];
						businesses.AuthUserName = userDetail.AuthUserName;
						businesses.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessesQueryResponse>(
					new GetAllDropboxesBusinessesQueryResponse
					{
						Businesses = businessesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"Businesses başarıyla getirildi.",
					$" Businesses başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"Businesses getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
