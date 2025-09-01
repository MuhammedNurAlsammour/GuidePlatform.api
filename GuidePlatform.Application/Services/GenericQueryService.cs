using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.Application.Operations;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Services
{
	/// <summary>
	/// Generic Query Service - Tüm query işlemleri için ortak servis
	/// Filtreleme, sayfalama ve Auth kullanıcı bilgilerini yönetir
	/// </summary>
	public class GenericQueryService
	{
		private readonly IAuthUserDetailService _authUserService;

		public GenericQueryService(IAuthUserDetailService authUserService)
		{
			_authUserService = authUserService;
		}

		/// <summary>
		/// Generic query işlemi - Filtreleme, sayfalama ve Auth bilgilerini içerir
		/// </summary>
		/// <typeparam name="TEntity">Entity tipi</typeparam>
		/// <typeparam name="TResponse">Response DTO tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="request">Base paginated query request</param>
		/// <param name="mapper">Entity'den Response DTO'ya dönüştürme fonksiyonu</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>TransactionResultPack</returns>
		public async Task<TransactionResultPack<List<TResponse>>> ExecuteQueryAsync<TEntity, TResponse>(
			IQueryable<TEntity> query,
			BasePaginatedQueryRequest request,
			Func<List<TEntity>, Dictionary<Guid, AuthUserDetailDto>, CancellationToken, Task<List<TResponse>>> mapper,
			CancellationToken cancellationToken = default) where TEntity : class
		{
			try
			{
				// Toplam sayıyı hesapla (filtreleme sonrası)
				var totalCountQuery = ApplyAuthFilters(query, request.GetAuthUserIdAsGuid(), request.GetAuthCustomerIdAsGuid());
				var totalCount = await totalCountQuery
					.AsNoTracking()
					.CountAsync(cancellationToken);

				// Verileri getir (filtreleme + sayfalama)
				var entities = await ApplyBaseFilters(query, request)
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				// Auth kullanıcı bilgilerini al
				var authUserDetails = await ExtractAuthUserDetailsAsync(entities, cancellationToken);

				// Response DTO'ları oluştur
				var responseData = await mapper(entities, authUserDetails, cancellationToken);
				// Başarılı sonuç döndürülüyor (responseData bir liste olduğu için generic tip List<TResponse> olarak kullanılmalı)
				return ResultFactory.CreateSuccessResult<List<TResponse>>(
					responseData,
					totalCount,
					totalCount,
					"İşlem Başarılı",
					"Veriler başarıyla getirildi.",
					$"Toplam kayıt sayısı: {totalCount}, sayfa: {request.GetValidatedPage()}, boyut: {request.GetValidatedSize()}."
				);

			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<List<TResponse>>(
					null,
					null,
					"Hata / İşlem Başarısız",
					"Veriler getirilirken bir hata oluştu.",
					ex.Message
				);
			}
		}

		/// <summary>
		/// Query'ye Auth filtrelerini uygular
		/// </summary>
		private IQueryable<T> ApplyAuthFilters<T>(IQueryable<T> query, Guid? authUserId = null, Guid? customerId = null) where T : class
		{
			if (authUserId.HasValue)
			{
				var property = typeof(T).GetProperty("AuthUserId");
				if (property != null)
				{
					query = query.Where(e => (Guid?)property.GetValue(e) == authUserId.Value);
				}
			}

			if (customerId.HasValue)
			{
				var property = typeof(T).GetProperty("CustomerId");
				if (property != null)
				{
					query = query.Where(e => (Guid?)property.GetValue(e) == customerId.Value);
				}
			}

			return query;
		}

		/// <summary>
		/// Query'ye sayfalama uygular
		/// </summary>
		private IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int page, int size)
		{
			var validatedPage = Math.Max(0, page);
			var validatedSize = Math.Max(1, Math.Min(100, size));

			return query.Skip(validatedPage * validatedSize).Take(validatedSize);
		}

		/// <summary>
		/// Temel filtreleme ve sayfalama işlemlerini uygular
		/// </summary>
		private IQueryable<T> ApplyBaseFilters<T>(IQueryable<T> query, BasePaginatedQueryRequest request) where T : class
		{
			// Auth filtrelerini uygula
			query = ApplyAuthFilters(query, request.GetAuthUserIdAsGuid(), request.GetAuthCustomerIdAsGuid());

			// Sayfalamayı uygula
			query = ApplyPagination(query, request.GetValidatedPage(), request.GetValidatedSize());

			return query;
		}

		/// <summary>
		/// Auth kullanıcı bilgilerini entity listesinden çıkarır
		/// </summary>
		private async Task<Dictionary<Guid, AuthUserDetailDto>> ExtractAuthUserDetailsAsync<T>(
			IEnumerable<T> entities,
			CancellationToken cancellationToken = default) where T : class
		{
			var authUserIds = entities
				.Where(e => GetAuthUserId(e) != null)
				.Select(e => GetAuthUserId(e)!.Value)
				.Distinct()
				.ToList();

			if (!authUserIds.Any())
				return new Dictionary<Guid, AuthUserDetailDto>();

			return await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);
		}

		/// <summary>
		/// Entity'den AuthUserId'yi çıkarır
		/// </summary>
		private static Guid? GetAuthUserId(object entity)
		{
			var property = entity.GetType().GetProperty("AuthUserId");
			return property?.GetValue(entity) as Guid?;
		}
	}
}
