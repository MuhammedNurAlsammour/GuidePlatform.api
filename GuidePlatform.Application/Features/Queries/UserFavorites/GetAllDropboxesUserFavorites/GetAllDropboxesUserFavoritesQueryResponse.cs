using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.UserFavorites.GetAllDropboxesUserFavorites
{
	public class GetAllDropboxesUserFavoritesQueryResponse
	{
		public List<userFavoritesDetailDto> userFavorites { get; set; } = new List<userFavoritesDetailDto>();
	}
	
	public class userFavoritesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
