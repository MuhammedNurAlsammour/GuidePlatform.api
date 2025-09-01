using Karmed.External.Auth.Library.Services;

namespace GuidePlatform.Application.Features.Queries.Base
{
	/// <summary>
	/// Tek bir entity getirmek için kullanılan base query request sınıfı
	/// ID özelliği ile birlikte gelir
	/// </summary>
	public abstract class BaseSingleQueryRequest : BaseQueryRequest
	{
		/// <summary>
		/// Getirilecek entity'nin ID'si
		/// </summary>
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
	}
}