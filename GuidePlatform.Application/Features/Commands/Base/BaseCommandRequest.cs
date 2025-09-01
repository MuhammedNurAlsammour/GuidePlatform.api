using Karmed.External.Auth.Library.Services;
using System.Text.Json;

namespace GuidePlatform.Application.Features.Commands.Base
{
	/// <summary>
	/// Base class for all command requests with safe GUID parsing utilities
	/// </summary>
	public abstract class BaseCommandRequest
	{
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public Guid? CreateUserId { get; set; }
		public Guid? UpdateUserId { get; set; }

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

		/// <summary>
		/// CreateUserId için otomatik token'dan alma - Yeni kayıt oluşturma için
		/// </summary>
		/// <param name="currentUserService">Current user service</param>
		/// <returns>Token'dan alınan UserId</returns>
		protected Guid? GetCreateUserIdFromToken(ICurrentUserService currentUserService)
		{
			// Önce request'ten gelen değeri kontrol et
			if (CreateUserId.HasValue)
			{
				return CreateUserId;
			}

			// Token'dan UserId'yi al
			var userIdFromService = currentUserService.GetUserId();
			if (!string.IsNullOrEmpty(userIdFromService) &&
				Guid.TryParse(userIdFromService, out Guid parsedUserId))
			{
				return parsedUserId;
			}

			return null;
		}

		/// <summary>
		/// UpdateUserId için otomatik token'dan alma - Güncelleme işlemleri için
		/// </summary>
		/// <param name="currentUserService">Current user service</param>
		/// <returns>Token'dan alınan UserId</returns>
		protected Guid? GetUpdateUserIdFromToken(ICurrentUserService currentUserService)
		{
			// Önce request'ten gelen değeri kontrol et
			if (UpdateUserId.HasValue)
			{
				return UpdateUserId;
			}

			// Token'dan UserId'yi al
			var userIdFromService = currentUserService.GetUserId();
			if (!string.IsNullOrEmpty(userIdFromService) &&
				Guid.TryParse(userIdFromService, out Guid parsedUserId))
			{
				return parsedUserId;
			}

			return null;
		}

		/// <summary>
		/// Yeni kayıt oluşturma için tüm auth bilgilerini otomatik doldur
		/// </summary>
		/// <param name="currentUserService">Current user service</param>
		/// <returns>Auth bilgileri tuple</returns>
		protected (Guid? CustomerId, Guid? UserId, Guid? CreateUserId, Guid? UpdateUserId) GetCreateAuthIds(ICurrentUserService currentUserService)
		{
			var (customerId, userId) = GetAuthIds(currentUserService);
			var createUserId = GetCreateUserIdFromToken(currentUserService);
			var updateUserId = GetUpdateUserIdFromToken(currentUserService);

			return (customerId, userId, createUserId, updateUserId);
		}

		/// <summary>
		/// Güncelleme işlemleri için auth bilgilerini otomatik doldur
		/// </summary>
		/// <param name="currentUserService">Current user service</param>
		/// <returns>Auth bilgileri tuple</returns>
		protected (Guid? CustomerId, Guid? UserId, Guid? UpdateUserId) GetUpdateAuthIds(ICurrentUserService currentUserService)
		{
			var (customerId, userId) = GetAuthIds(currentUserService);
			var updateUserId = GetUpdateUserIdFromToken(currentUserService);

			return (customerId, userId, updateUserId);
		}

		/// <summary>
		/// JWT Token'dan UserId ve CustomerId'yi parse et - Debug için
		/// </summary>
		/// <param name="jwtToken">JWT token string</param>
		/// <returns>UserId ve CustomerId tuple</returns>
		protected (Guid? UserId, Guid? CustomerId) ParseJwtTokenForDebug(string jwtToken)
		{
			try
			{
				// JWT token'ı parçala (header.payload.signature)
				var parts = jwtToken.Split('.');
				if (parts.Length != 3)
				{
					return (null, null);
				}

				// Payload'ı decode et
				var payload = parts[1];
				var paddedPayload = payload.PadRight(4 * ((payload.Length + 3) / 4), '=');
				var decodedPayload = Convert.FromBase64String(paddedPayload.Replace('-', '+').Replace('_', '/'));
				var jsonPayload = System.Text.Encoding.UTF8.GetString(decodedPayload);

				// JSON'dan UserData'yı al
				using var document = JsonDocument.Parse(jsonPayload);
				var root = document.RootElement;

				if (root.TryGetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata", out var userDataElement))
				{
					var userDataJson = userDataElement.GetString();
					if (!string.IsNullOrEmpty(userDataJson))
					{
						using var userDataDoc = JsonDocument.Parse(userDataJson);
						var userData = userDataDoc.RootElement;

						Guid? userId = null;
						Guid? customerId = null;

						if (userData.TryGetProperty("UserId", out var userIdElement))
						{
							var userIdStr = userIdElement.GetString();
							if (Guid.TryParse(userIdStr, out Guid parsedUserId))
							{
								userId = parsedUserId;
							}
						}

						if (userData.TryGetProperty("CustomerId", out var customerIdElement))
						{
							var customerIdStr = customerIdElement.GetString();
							if (Guid.TryParse(customerIdStr, out Guid parsedCustomerId))
							{
								customerId = parsedCustomerId;
							}
						}

						return (userId, customerId);
					}
				}
			}
			catch (Exception ex)
			{
				// Log error for debugging
				System.Diagnostics.Debug.WriteLine($"JWT Token parse error: {ex.Message}");
			}

			return (null, null);
		}
	}
}
