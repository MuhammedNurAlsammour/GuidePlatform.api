using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetAllDropboxesBusinessServices
{
	// Bu handler, businessServices dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessServicesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessServicesQueryRequest, TransactionResultPack<GetAllDropboxesBusinessServicesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessServicesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessServicesQueryResponse>> Handle(GetAllDropboxesBusinessServicesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessServicesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessServices
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessServicess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessServicess == null || businessServicess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessServicesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessServices Bulunamadı",
						"Belirtilen müşteriye ait businessServices bulunamadı.",
						$"No businessServices found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessServicesDetail = businessServicess.Select(businessServices => new businessServicesDetailDto
				{
					Id = businessServices.Id,
					AuthCustomerId = businessServices.AuthCustomerId,
					AuthUserId = businessServices.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessServicess.Select(businessServices => businessServices.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessServices için kullanıcı bilgilerini ekle
				foreach (var businessServices in businessServicesDetail)
				{
					if (businessServices.AuthUserId.HasValue && authUserDetails.ContainsKey(businessServices.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessServices.AuthUserId.Value];
						businessServices.AuthUserName = userDetail.AuthUserName;
						businessServices.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessServicesQueryResponse>(
					new GetAllDropboxesBusinessServicesQueryResponse
					{
						businessServices = businessServicesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessServices başarıyla getirildi.",
					$" businessServices başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessServicesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessServices getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
