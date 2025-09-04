using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllDropboxesBusinessImages
{
	// Bu handler, businessImages dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessImagesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessImagesQueryRequest, TransactionResultPack<GetAllDropboxesBusinessImagesQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessImagesQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessImagesQueryResponse>> Handle(GetAllDropboxesBusinessImagesQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessImagesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessImages
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessImagess = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessImagess == null || businessImagess.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessImagesQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessImages Bulunamadı",
						"Belirtilen müşteriye ait businessImages bulunamadı.",
						$"No businessImages found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessImagesDetail = businessImagess.Select(businessImages => new businessImagesDetailDto
				{
					Id = businessImages.Id,
					AuthCustomerId = businessImages.AuthCustomerId,
					AuthUserId = businessImages.AuthUserId,
                    BusinessId = businessImages.BusinessId,
                    Photo = businessImages.Photo != null ? Convert.ToBase64String(businessImages.Photo) : null,
                    Thumbnail = businessImages.Thumbnail != null ? Convert.ToBase64String(businessImages.Thumbnail) : null,
                    PhotoContentType = businessImages.PhotoContentType,
                    AltText = businessImages.AltText,
                    IsPrimary = businessImages.IsPrimary,
                    SortOrder = businessImages.SortOrder,
                    Icon = businessImages.Icon,
                    ImageType = businessImages.ImageType,
                    // Diğer özellikler buraya eklenebilir
                }).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessImagess.Select(businessImages => businessImages.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessImages için kullanıcı bilgilerini ekle
				foreach (var businessImages in businessImagesDetail)
				{
					if (businessImages.AuthUserId.HasValue && authUserDetails.ContainsKey(businessImages.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessImages.AuthUserId.Value];
						businessImages.AuthUserName = userDetail.AuthUserName;
						businessImages.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessImagesQueryResponse>(
					new GetAllDropboxesBusinessImagesQueryResponse
					{
						businessImages = businessImagesDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessImages başarıyla getirildi.",
					$" businessImages başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessImagesQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessImages getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
