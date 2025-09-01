using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Dtos;
using Karmed.External.Auth.Library.Contexts;
using Karmed.External.Auth.Library.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuidePlatform.Application.Services
{
	/// <summary>
	/// Auth kullanıcı detay bilgileri için servis
	/// Username ve CustomerName bilgilerini toplu olarak getirir
	/// </summary>
	public class AuthUserDetailService : IAuthUserDetailService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly AuthDbContext _authContext;

		public AuthUserDetailService(UserManager<AppUser> userManager, AuthDbContext authContext)
		{
			_userManager = userManager;
			_authContext = authContext;
		}

		/// <summary>
		/// Verilen AuthUserId listesi için kullanıcı detaylarını getirir
		/// Performans için bulk operation kullanır
		/// </summary>
		public async Task<Dictionary<Guid, AuthUserDetailDto>> GetAuthUserDetailsAsync(
			List<Guid> authUserIds,
			CancellationToken cancellationToken = default)
		{
			if (!authUserIds.Any())
				return new Dictionary<Guid, AuthUserDetailDto>();

			try
			{
				// 1️⃣ AspNetUsers'dan UserName ve CustomerId'yi al
				var usersWithCustomerIds = await _userManager.Users
					.Where(u => authUserIds.Contains(u.Id))
					.Select(u => new
					{
						u.Id,
						AuthUserName = u.UserName,
						CustomerId = u.CustomerId
					})
					.ToListAsync(cancellationToken);

				if (!usersWithCustomerIds.Any())
					return new Dictionary<Guid, AuthUserDetailDto>();

				// 2️⃣ Customers'dan Name'leri al
				var customerIds = usersWithCustomerIds
					.Select(u => u.CustomerId)
					.Distinct()
					.ToList();

				var customerNames = await _authContext.Customers
					.Where(c => customerIds.Contains(c.Id) && c.RowIsActive && !c.RowIsDeleted)
					.ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

				// 3️⃣ Sonuçları birleştir
				var result = new Dictionary<Guid, AuthUserDetailDto>();

				foreach (var user in usersWithCustomerIds)
				{
					var customerName = customerNames.ContainsKey(user.CustomerId)
						? customerNames[user.CustomerId]
						: null;

					result[user.Id] = new AuthUserDetailDto
					{
						AuthUserId = user.Id,
						AuthUserName = user.AuthUserName,
						AuthCustomerName = customerName,  // Customers.Name
						CustomerId = user.CustomerId
					};
				}

				return result;
			}
			catch (Exception)
			{
				// Hata durumunda boş dictionary döndür
				return new Dictionary<Guid, AuthUserDetailDto>();
			}
		}

		/// <summary>
		/// Tek bir AuthUserId için kullanıcı detaylarını getirir
		/// </summary>
		public async Task<AuthUserDetailDto?> GetAuthUserDetailAsync(
			Guid authUserId,
			CancellationToken cancellationToken = default)
		{
			var results = await GetAuthUserDetailsAsync(new List<Guid> { authUserId }, cancellationToken);
			return results.ContainsKey(authUserId) ? results[authUserId] : null;
		}
	}
}