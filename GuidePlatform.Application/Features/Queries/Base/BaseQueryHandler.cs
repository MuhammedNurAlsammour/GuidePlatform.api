using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos;
using Karmed.External.Auth.Library.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GuidePlatform.Application.Features.Queries.Base
{
	/// <summary>
	/// Base class for all query handlers with safe GUID parsing utilities
	/// </summary>
	public abstract class BaseQueryHandler
	{
		protected readonly ICurrentUserService _currentUserService;
		protected readonly IAuthUserDetailService _authUserService;

		protected BaseQueryHandler(ICurrentUserService currentUserService, IAuthUserDetailService authUserService)
		{
			_currentUserService = currentUserService;
			_authUserService = authUserService;
		}

		/// <summary>
		/// Token'dan UserId ve CustomerId'yi güvenli bir şekilde alır
		/// </summary>
		/// <param name="requestUserId">Request'ten gelen UserId</param>
		/// <param name="requestCustomerId">Request'ten gelen CustomerId</param>
		/// <returns>UserId ve CustomerId tuple</returns>
		protected (Guid? UserId, Guid? CustomerId) GetAuthIdsFromToken(Guid? requestUserId = null, Guid? requestCustomerId = null)
		{
			var userId = requestUserId;
			var customerId = requestCustomerId;

			// Eğer request'te yoksa token'dan al
			if (!userId.HasValue || !customerId.HasValue)
			{
				// UserId'yi al
				if (!userId.HasValue)
				{
					var userIdFromService = _currentUserService?.GetUserId();
					if (!string.IsNullOrEmpty(userIdFromService) &&
						Guid.TryParse(userIdFromService, out Guid parsedUserId))
					{
						userId = parsedUserId;
					}
				}

				// CustomerId'yi al
				if (!customerId.HasValue)
				{
					var customerIdFromService = _currentUserService?.GetCustomerId();
					if (!string.IsNullOrEmpty(customerIdFromService) &&
						Guid.TryParse(customerIdFromService, out Guid parsedCustomerId))
					{
						customerId = parsedCustomerId;
					}
				}
			}

			return (userId, customerId);
		}

		/// <summary>
		/// Güvenli GUID dönüşümü için utility method
		/// </summary>
		/// <param name="requestValue">Request'ten gelen değer</param>
		/// <param name="serviceMethod">Service'den değer almak için method</param>
		/// <returns>Parsed GUID veya null</returns>
		protected Guid? GetSafeGuid(Guid? requestValue, Func<string?> serviceMethod)
		{
			// Önce request'ten gelen değerleri kontrol et
			if (requestValue.HasValue)
			{
				return requestValue;
			}

			// Service'den al ve parse et
			var serviceValue = serviceMethod?.Invoke();
			if (!string.IsNullOrEmpty(serviceValue) &&
				Guid.TryParse(serviceValue, out Guid parsedValue))
			{
				return parsedValue;
			}

			return null;
		}

		/// <summary>
		/// UserId için güvenli GUID dönüşümü
		/// </summary>
		protected Guid? GetSafeUserId(Guid? requestUserId = null)
		{
			return GetSafeGuid(requestUserId, () => _currentUserService?.GetUserId());
		}

		/// <summary>
		/// CustomerId için güvenli GUID dönüşümü
		/// </summary>
		protected Guid? GetSafeCustomerId(Guid? requestCustomerId = null)
		{
			return GetSafeGuid(requestCustomerId, () => _currentUserService?.GetCustomerId());
		}

		/// <summary>
		/// Auth kullanıcı bilgilerini toplu olarak getirir
		/// </summary>
		/// <param name="authUserIds">AuthUserId listesi</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Auth kullanıcı detayları dictionary</returns>
		protected async Task<Dictionary<Guid, AuthUserDetailDto>> GetAuthUserDetailsAsync(
			List<Guid> authUserIds,
			CancellationToken cancellationToken = default)
		{
			if (!authUserIds.Any())
				return new Dictionary<Guid, AuthUserDetailDto>();

			return await _authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);
		}

		/// <summary>
		/// Tek bir Auth kullanıcı bilgisini getirir
		/// </summary>
		/// <param name="authUserId">AuthUserId</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Auth kullanıcı detayı</returns>
		protected async Task<AuthUserDetailDto?> GetAuthUserDetailAsync(
			Guid authUserId,
			CancellationToken cancellationToken = default)
		{
			return await _authUserService.GetAuthUserDetailAsync(authUserId, cancellationToken);
		}

		/// <summary>
		/// Auth kullanıcı bilgilerini BaseEntity'den türeyen entity'den çıkarır
		/// </summary>
		/// <typeparam name="T">BaseEntity'den türeyen entity tipi</typeparam>
		/// <param name="entities">Entity listesi</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Auth kullanıcı detayları dictionary</returns>
		protected async Task<Dictionary<Guid, AuthUserDetailDto>> ExtractAuthUserDetailsAsync<T>(
			IEnumerable<T> entities,
			CancellationToken cancellationToken = default) where T : GuidePlatform.Domain.Entities.Common.BaseEntity
		{
			var authUserIds = entities
				.Where(e => e.AuthUserId.HasValue)
				.Select(e => e.AuthUserId!.Value)
				.Distinct()
				.ToList();

			return await GetAuthUserDetailsAsync(authUserIds, cancellationToken);
		}

		/// <summary>
		/// Auth kullanıcı bilgilerini anonymous type veya custom object'den çıkarır (reflection kullanır)
		/// </summary>
		/// <typeparam name="T">Herhangi bir tip (AuthUserId property'si olmalı)</typeparam>
		/// <param name="entities">Entity listesi</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Auth kullanıcı detayları dictionary</returns>
		protected async Task<Dictionary<Guid, AuthUserDetailDto>> ExtractAuthUserDetailsFromObjectsAsync<T>(
			IEnumerable<T> entities,
			CancellationToken cancellationToken = default) where T : class
		{
			var authUserIds = entities
				.Where(e => GetAuthUserId(e) != null)
				.Select(e => GetAuthUserId(e)!.Value)
				.Distinct()
				.ToList();

			return await GetAuthUserDetailsAsync(authUserIds, cancellationToken);
		}

		/// <summary>
		/// Entity'den AuthUserId'yi çıkarır (reflection kullanır)
		/// </summary>
		/// <param name="entity">Entity objesi</param>
		/// <returns>AuthUserId veya null</returns>
		private static Guid? GetAuthUserId(object entity)
		{
			var property = entity.GetType().GetProperty("AuthUserId");
			return property?.GetValue(entity) as Guid?;
		}

		/// <summary>
		/// Entity ve auth user details'den auth bilgilerini çıkarır
		/// </summary>
		/// <param name="entity">BaseEntity'den türeyen entity</param>
		/// <param name="authUserDetails">Auth user details dictionary</param>
		/// <returns>AuthUserName ve AuthCustomerName tuple</returns>
		protected static (string? AuthUserName, string? AuthCustomerName) GetAuthUserInfo(
			GuidePlatform.Domain.Entities.Common.BaseEntity entity,
			Dictionary<Guid, AuthUserDetailDto> authUserDetails)
		{
			string? authUserName = null;
			string? authCustomerName = null;

			if (entity.AuthUserId.HasValue && authUserDetails.ContainsKey(entity.AuthUserId.Value))
			{
				var userDetail = authUserDetails[entity.AuthUserId.Value];
				authUserName = userDetail.AuthUserName;
				authCustomerName = userDetail.AuthCustomerName;
			}

			return (authUserName, authCustomerName);
		}

		/// <summary>
		/// Query'ye sayfalama uygular
		/// </summary>
		/// <typeparam name="T">Entity tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="page">Sayfa numarası</param>
		/// <param name="size">Sayfa boyutu</param>
		/// <returns>Sayfalanmış query</returns>
		protected IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int page, int size)
			where T : GuidePlatform.Domain.Entities.Common.BaseEntity
		{
			var validatedPage = Math.Max(0, page);
			var validatedSize = Math.Max(1, Math.Min(100, size));

			// Entity Framework uyarısını önlemek için OrderBy ekle
			query = query.OrderByDescending(e => e.RowCreatedDate);

			return query.Skip(validatedPage * validatedSize).Take(validatedSize);
		}

		/// <summary>
		/// Query'ye Auth filtrelerini uygular - BaseEntity'den türeyen sınıflar için
		/// </summary>
		/// <typeparam name="T">BaseEntity'den türeyen entity tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="authUserId">AuthUserId (opsiyonel)</param>
		/// <param name="authCustomerId">AuthCustomerId (opsiyonel)</param>
		/// <returns>Filtrelenmiş query</returns>
		protected IQueryable<T> ApplyAuthFilters<T>(IQueryable<T> query, Guid? authUserId = null, Guid? authCustomerId = null)
			where T : GuidePlatform.Domain.Entities.Common.BaseEntity
		{
			if (authUserId.HasValue)
			{
				query = query.Where(e => e.AuthUserId == authUserId.Value);
			}

			if (authCustomerId.HasValue)
			{
				query = query.Where(e => e.AuthCustomerId == authCustomerId.Value);
			}

			return query;
		}

		/// <summary>
		/// Temel filtreleme ve sayfalama işlemlerini uygular
		/// </summary>
		/// <typeparam name="T">Entity tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="request">Base paginated query request</param>
		/// <returns>Filtrelenmiş ve sayfalanmış query</returns>
		protected IQueryable<T> ApplyBaseFilters<T>(IQueryable<T> query, BasePaginatedQueryRequest request)
			where T : GuidePlatform.Domain.Entities.Common.BaseEntity
		{
			var authUserId = GetSafeUserId(request.AuthUserId);
			var authCustomerId = GetSafeCustomerId(request.AuthCustomerId);

			// Auth filtrelerini uygula
			query = ApplyAuthFilters(query, authUserId, authCustomerId);

			// Sayfalamayı uygula
			query = ApplyPagination(query, request.GetValidatedPage(), request.GetValidatedSize());

			return query;
		}
	}
}
