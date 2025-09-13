using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.JobSeekers.GetAllDropboxesJobSeekers
{
	// Bu handler, jobSeekers dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesJobSeekersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesJobSeekersQueryRequest, TransactionResultPack<GetAllDropboxesJobSeekersQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesJobSeekersQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesJobSeekersQueryResponse>> Handle(GetAllDropboxesJobSeekersQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesJobSeekersQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.jobSeekers
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var jobSeekerss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (jobSeekerss == null || jobSeekerss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesJobSeekersQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / jobSeekers Bulunamadı",
						"Belirtilen müşteriye ait jobSeekers bulunamadı.",
						$"No jobSeekers found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var jobSeekersDetail = jobSeekerss.Select(jobSeekers => new jobSeekersDetailDto
				{
					Id = jobSeekers.Id,
					AuthCustomerId = jobSeekers.AuthCustomerId,
					AuthUserId = jobSeekers.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = jobSeekerss.Select(jobSeekers => jobSeekers.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her jobSeekers için kullanıcı bilgilerini ekle
				foreach (var jobSeekers in jobSeekersDetail)
				{
					if (jobSeekers.AuthUserId.HasValue && authUserDetails.ContainsKey(jobSeekers.AuthUserId.Value))
					{
						var userDetail = authUserDetails[jobSeekers.AuthUserId.Value];
						jobSeekers.AuthUserName = userDetail.AuthUserName;
						jobSeekers.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesJobSeekersQueryResponse>(
					new GetAllDropboxesJobSeekersQueryResponse
					{
						jobSeekers = jobSeekersDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"jobSeekers başarıyla getirildi.",
					$" jobSeekers başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesJobSeekersQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"jobSeekers getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
