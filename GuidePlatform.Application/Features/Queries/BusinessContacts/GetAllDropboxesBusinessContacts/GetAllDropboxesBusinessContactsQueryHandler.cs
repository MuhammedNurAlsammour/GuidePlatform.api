using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllDropboxesBusinessContacts
{
	// Bu handler, businessContacts dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesBusinessContactsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesBusinessContactsQueryRequest, TransactionResultPack<GetAllDropboxesBusinessContactsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesBusinessContactsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesBusinessContactsQueryResponse>> Handle(GetAllDropboxesBusinessContactsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessContactsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.businessContacts
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var businessContactss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (businessContactss == null || businessContactss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessContactsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / businessContacts Bulunamadı",
						"Belirtilen müşteriye ait businessContacts bulunamadı.",
						$"No businessContacts found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var businessContactsDetail = businessContactss.Select(businessContacts => new businessContactsDetailDto
				{
					Id = businessContacts.Id,
					AuthCustomerId = businessContacts.AuthCustomerId,
					AuthUserId = businessContacts.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = businessContactss.Select(businessContacts => businessContacts.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her businessContacts için kullanıcı bilgilerini ekle
				foreach (var businessContacts in businessContactsDetail)
				{
					if (businessContacts.AuthUserId.HasValue && authUserDetails.ContainsKey(businessContacts.AuthUserId.Value))
					{
						var userDetail = authUserDetails[businessContacts.AuthUserId.Value];
						businessContacts.AuthUserName = userDetail.AuthUserName;
						businessContacts.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesBusinessContactsQueryResponse>(
					new GetAllDropboxesBusinessContactsQueryResponse
					{
						businessContacts = businessContactsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"businessContacts başarıyla getirildi.",
					$" businessContacts başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesBusinessContactsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"businessContacts getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
