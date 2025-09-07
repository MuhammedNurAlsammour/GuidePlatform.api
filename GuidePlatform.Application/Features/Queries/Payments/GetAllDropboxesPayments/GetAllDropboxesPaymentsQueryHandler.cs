using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos.ResponseDtos.Payments;
using GuidePlatform.Application.Features.Queries.Base;
using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Payments.GetAllDropboxesPayments
{
	// Bu handler, payments dropdown için getirir. Clean Architecture prensiplerine uygun olarak yazılmıştır.
	public class GetAllDropboxesPaymentsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDropboxesPaymentsQueryRequest, TransactionResultPack<GetAllDropboxesPaymentsQueryResponse>>
	{
		private readonly IApplicationDbContext _context;

		public GetAllDropboxesPaymentsQueryHandler(
			IApplicationDbContext context,
			IAuthUserDetailService authUserService,
			ICurrentUserService currentUserService) : base(currentUserService, authUserService)
		{
			_context = context;
		}

		public async Task<TransactionResultPack<GetAllDropboxesPaymentsQueryResponse>> Handle(GetAllDropboxesPaymentsQueryRequest request, CancellationToken cancellationToken)
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
					return ResultFactory.CreateErrorResult<GetAllDropboxesPaymentsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / Eksik Parametre",
						"Müşteri ID'si belirtilmedi ve token'da bulunamadı.",
						"Customer ID parameter is required and not found in token."
					);
				}

				// Temel sorgu oluşturuluyor
				var baseQuery = _context.payments
					.Where(x => x.RowIsActive && !x.RowIsDeleted);

				// Yetkilendirme filtreleri uygulanıyor
				var filteredQuery = ApplyAuthFilters(baseQuery, null, authCustomerId);

				// Sayfalama uygulanıyor
				var paymentss = await ApplyPagination(filteredQuery, request.Page, request.Size)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (paymentss == null || paymentss.Count == 0)
				{
					return ResultFactory.CreateErrorResult<GetAllDropboxesPaymentsQueryResponse>(
						request.AuthCustomerId,
						null,
						"Hata / payments Bulunamadı",
						"Belirtilen müşteriye ait payments bulunamadı.",
						$"No payments found for customer ID: {authCustomerId}"
					);
				}

				// Dropdown için sadece gerekli alanları seçiyoruz
				var paymentsDetail = paymentss.Select(payments => new paymentsDetailDto
				{
					Id = payments.Id,
					AuthCustomerId = payments.AuthCustomerId,
					AuthUserId = payments.AuthUserId
					// Diğer özellikler buraya eklenebilir
				}).ToList();

				// 🎯 Auth kullanıcı bilgilerini service ile al
				var authUserIds = paymentss.Select(payments => payments.AuthUserId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
				var authUserDetails = await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);

				// Her payments için kullanıcı bilgilerini ekle
				foreach (var payments in paymentsDetail)
				{
					if (payments.AuthUserId.HasValue && authUserDetails.ContainsKey(payments.AuthUserId.Value))
					{
						var userDetail = authUserDetails[payments.AuthUserId.Value];
						payments.AuthUserName = userDetail.AuthUserName;
						payments.AuthCustomerName = userDetail.AuthCustomerName;
					}
				}

				return ResultFactory.CreateSuccessResult<GetAllDropboxesPaymentsQueryResponse>(
					new GetAllDropboxesPaymentsQueryResponse
					{
						payments = paymentsDetail
					},
					authCustomerId,
					null,
					"İşlem Başarılı",
					"payments başarıyla getirildi.",
					$" payments başarıyla getirildi."
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<GetAllDropboxesPaymentsQueryResponse>(
					request.AuthCustomerId,
					null,
					"Hata / İşlem Başarısız",
					"payments getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}
}
