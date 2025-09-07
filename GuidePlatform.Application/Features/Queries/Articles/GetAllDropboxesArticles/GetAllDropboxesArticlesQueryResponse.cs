using E = GuidePlatform.Domain.Entities;

namespace GuidePlatform.Application.Features.Queries.Articles.GetAllDropboxesArticles
{
	public class GetAllDropboxesArticlesQueryResponse
	{
		public List<articlesDetailDto> articles { get; set; } = new List<articlesDetailDto>();
	}
	
	public class articlesDetailDto
	{
		public Guid Id { get; set; }
		public Guid? AuthCustomerId { get; set; }
		public Guid? AuthUserId { get; set; }
		public string? AuthUserName { get; set; }
		public string? AuthCustomerName { get; set; }
		// Diğer özellikler buraya eklenebilir
	}
}
