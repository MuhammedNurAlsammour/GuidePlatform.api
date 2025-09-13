using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllDropboxesJobOpportunities
{
	// Bu handler, jobOpportunities dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesJobOpportunitiesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesJobOpportunitiesQueryRequest, TransactionResultPack<GetAllDropboxesJobOpportunitiesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesJobOpportunitiesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesJobOpportunitiesQueryResponse>> Handle(GetAllDropboxesJobOpportunitiesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesJobOpportunitiesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.jobOpportunities
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var jobOpportunitiess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (jobOpportunitiess == null || jobOpportunitiess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesJobOpportunitiesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / jobOpportunities Bulunamadı",
						"Belirtilen müşteriye ait jobOpportunities bulunamadı.",
						$"No jobOpportunities found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var jobOpportunitiesDetail = jobOpportunitiess.Select(jobOpportunities => new jobOpportunitiesDetailDto
				{
					Id = jobOpportunities.Id,
					AuthCustomerId = jobOpportunities.AuthCustomerId,
					AuthUserId = jobOpportunities.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = jobOpportunitiess.Select(jobOpportunities => jobOpportunities.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her jobOpportunities için kullanıcı bilgilerini ekle
				foreach (var jobOpportunities in jobOpportunitiesDetail)
				{
					if (jobOpportunities.AuthUserId.HasValue && authUserDetails.ContainsKey(jobOpportunities.AuthUserId.Value))
					{
						var userDetail = authUserDetails[jobOpportunities.AuthUserId.Value];
						jobOpportunities.AuthUserName = userDetail.AuthUserName;
						jobOpportunities.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesJobOpportunitiesQueryResponse>(
					new GetAllDropboxesJobOpportunitiesQueryResponse
					{
						jobOpportunities = jobOpportunitiesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"jobOpportunities başarıyla getirildi.",
					$" jobOpportunities başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesJobOpportunitiesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"jobOpportunities getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
