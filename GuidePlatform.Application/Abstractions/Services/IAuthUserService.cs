using GuidePlatform.Application.Dtos;

namespace GuidePlatform.Application.Abstractions.Services
{
	/// <summary>
	/// Auth kullanıcı detay bilgileri için servis interface
	/// Username ve CustomerName bilgilerini toplu olarak getirir
	/// </summary>
	public interface IAuthUserDetailService
	{
		/// <summary>
		/// Verilen AuthUserId listesi için kullanıcı detaylarını getirir
		/// </summary>
		/// <param name="authUserIds">AuthUserId listesi</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>AuthUserId -> AuthUserDetailDto dictionary</returns>
		Task<Dictionary<Guid, AuthUserDetailDto>> GetAuthUserDetailsAsync(
			List<Guid> authUserIds,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Tek bir AuthUserId için kullanıcı detaylarını getirir
		/// </summary>
		/// <param name="authUserId">AuthUserId</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>AuthUserDetailDto veya null</returns>
		Task<AuthUserDetailDto?> GetAuthUserDetailAsync(
			Guid authUserId,
			CancellationToken cancellationToken = default);
	}
}