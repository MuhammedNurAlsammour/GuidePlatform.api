using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Commands.Base
{
	/// <summary>
	/// Base class for all command handlers with safe GUID parsing utilities
	/// </summary>
	public abstract class BaseCommandHandler
	{
		protected readonly ICurrentUserService _currentUserService;

		protected BaseCommandHandler(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
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
	}
}