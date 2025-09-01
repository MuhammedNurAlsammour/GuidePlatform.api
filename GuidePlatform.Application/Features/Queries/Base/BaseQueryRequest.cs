using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Base
{
	/// <summary>
	/// Base class for all query requests with safe GUID parsing utilities
	/// </summary>
	public abstract class BaseQueryRequest
	{
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// ID'yi Guid'e dönüştürür
		/// </summary>
		public Guid? GetIdAsGuid()
		{
			if (string.IsNullOrEmpty(Id))
				return null;

			return Guid.TryParse(Id, out var result) ? result : null;
		}

		/// <summary>
		/// AuthUserId'yi Guid'e dönüştürür
		/// </summary>
		public Guid? GetAuthUserIdAsGuid()
		{
			return AuthUserId;
		}

		/// <summary>
		/// AuthCustomerId'yi Guid'e dönüştürür
		/// </summary>
		public Guid? GetAuthCustomerIdAsGuid()
		{
			return AuthCustomerId;
		}

		/// <summary>
		/// Güvenli GUID dönüşümü için utility method
		/// </summary>
		/// <param name="currentUserService">Current user service</param>
		/// <returns>UserId ve CustomerId tuple</returns>
		protected (Guid? CustomerId, Guid? UserId) GetAuthIds(ICurrentUserService currentUserService)
		{
			// Güvenli GUID dönüşümü - null kontrolü ve hata yönetimi
			Guid? customerId = null;
			Guid? userId = null;

			// Önce request'ten gelen değerleri kontrol et
			if (AuthCustomerId.HasValue)
			{
				customerId = AuthCustomerId;
			}
			else if (!string.IsNullOrEmpty(currentUserService.GetCustomerId()) &&
					 Guid.TryParse(currentUserService.GetCustomerId(), out Guid parsedCustomerId))
			{
				customerId = parsedCustomerId;
			}

			if (AuthUserId.HasValue)
			{
				userId = AuthUserId;
			}
			else if (!string.IsNullOrEmpty(currentUserService.GetUserId()) &&
					 Guid.TryParse(currentUserService.GetUserId(), out Guid parsedUserId))
			{
				userId = parsedUserId;
			}

			return (customerId, userId);
		}

		/// <summary>
		/// UserId için güvenli GUID dönüşümü
		/// </summary>
		protected Guid? GetSafeUserId(ICurrentUserService currentUserService)
		{
			if (AuthUserId.HasValue)
			{
				return AuthUserId;
			}

			var userIdFromService = currentUserService.GetUserId();
			if (!string.IsNullOrEmpty(userIdFromService) &&
				Guid.TryParse(userIdFromService, out Guid parsedUserId))
			{
				return parsedUserId;
			}

			return null;
		}

		/// <summary>
		/// CustomerId için güvenli GUID dönüşümü
		/// </summary>
		protected Guid? GetSafeCustomerId(ICurrentUserService currentUserService)
		{
			if (AuthCustomerId.HasValue)
			{
				return AuthCustomerId;
			}

			var customerIdFromService = currentUserService.GetCustomerId();
			if (!string.IsNullOrEmpty(customerIdFromService) &&
				Guid.TryParse(customerIdFromService, out Guid parsedCustomerId))
			{
				return parsedCustomerId;
			}

			return null;
		}
	}
}
