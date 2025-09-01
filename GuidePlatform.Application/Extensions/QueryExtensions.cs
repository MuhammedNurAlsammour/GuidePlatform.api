using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Extensions
{
	/// <summary>
	/// Query işlemleri için extension methods
	/// Ortak filtreleme ve sayfalama işlemlerini kolaylaştırır
	/// </summary>
	public static class QueryExtensions
	{
		/// <summary>
		/// Base query'ye AuthUserId ve authCustomerId filtrelerini ekler
		/// </summary>
		/// <typeparam name="T">Entity tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="authUserId">AuthUserId (opsiyonel)</param>
		/// <param name="authcustomerId">authCustomerId (opsiyonel)</param>
		/// <returns>Filtrelenmiş query</returns>
		public static IQueryable<T> ApplyAuthFilters<T>(
			this IQueryable<T> query,
			Guid? authUserId = null,
			Guid? authcustomerId = null) where T : class
		{
			if (authUserId.HasValue)
			{
				var property = typeof(T).GetProperty("AuthUserId");
				if (property != null)
				{
					query = query.Where(e => (Guid?)property.GetValue(e) == authUserId.Value);
				}
			}

			if (authcustomerId.HasValue)
			{
				var property = typeof(T).GetProperty("CustomerId");
				if (property != null)
				{
					query = query.Where(e => (Guid?)property.GetValue(e) == authcustomerId.Value);
				}
			}

			return query;
		}

		/// <summary>
		/// Query'ye sayfalama uygular
		/// </summary>
		/// <typeparam name="T">Entity tipi</typeparam>
		/// <param name="query">Base query</param>
		/// <param name="page">Sayfa numarası</param>
		/// <param name="size">Sayfa boyutu</param>
		/// <returns>Sayfalanmış query</returns>
		public static IQueryable<T> ApplyPagination<T>(
			this IQueryable<T> query,
			int page,
			int size)
		{
			return query.Skip(page * size).Take(size);
		}

		/// <summary>
		/// Auth kullanıcı bilgilerini entity listesinden çıkarır
		/// </summary>
		/// <typeparam name="T">Entity tipi</typeparam>
		/// <param name="entities">Entity listesi</param>
		/// <param name="authUserService">Auth user service</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Auth kullanıcı detayları dictionary</returns>
		public static async Task<Dictionary<Guid, AuthUserDetailDto>> ExtractAuthUserDetailsAsync<T>(
			this IEnumerable<T> entities,
			IAuthUserDetailService authUserService,
			CancellationToken cancellationToken = default) where T : class
		{
			var authUserIds = entities
				.Where(e => GetAuthUserId(e) != null)
				.Select(e => GetAuthUserId(e)!.Value)
				.Distinct()
				.ToList();

			if (!authUserIds.Any())
				return new Dictionary<Guid, AuthUserDetailDto>();

			return await authUserService.GetAuthUserDetailsAsync(authUserIds, cancellationToken);
		}

		/// <summary>
		/// Entity'den AuthUserId'yi çıkarır
		/// </summary>
		/// <param name="entity">Entity objesi</param>
		/// <returns>AuthUserId veya null</returns>
		private static Guid? GetAuthUserId(object entity)
		{
			var property = entity.GetType().GetProperty("AuthUserId");
			return property?.GetValue(entity) as Guid?;
		}
	}
}
