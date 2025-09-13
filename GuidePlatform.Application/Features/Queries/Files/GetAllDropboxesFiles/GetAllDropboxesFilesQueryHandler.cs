using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Files;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllDropboxesFiles
{
	// Bu handler, files dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesFilesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesFilesQueryRequest, TransactionResultPack<GetAllDropboxesFilesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesFilesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesFilesQueryResponse>> Handle(GetAllDropboxesFilesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesFilesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.files
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var filess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (filess == null || filess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesFilesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / files Bulunamadı",
						"Belirtilen müşteriye ait files bulunamadı.",
						$"No files found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var filesDetail = filess.Select(files => new filesDetailDto
				{
					Id = files.Id,
					AuthCustomerId = files.AuthCustomerId,
					AuthUserId = files.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = filess.Select(files => files.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her files için kullanıcı bilgilerini ekle
				foreach (var files in filesDetail)
				{
					if (files.AuthUserId.HasValue && authUserDetails.ContainsKey(files.AuthUserId.Value))
					{
						var userDetail = authUserDetails[files.AuthUserId.Value];
						files.AuthUserName = userDetail.AuthUserName;
						files.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesFilesQueryResponse>(
					new GetAllDropboxesFilesQueryResponse
					{
						files = filesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"files başarıyla getirildi.",
					$" files başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesFilesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"files getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
