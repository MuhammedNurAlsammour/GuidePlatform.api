using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Articles; namespace
GuidePlatform.Application.Features.Queries.Articles.GetArticlesById
{ public class
GetArticlesByIdQueryResponse { public int TotalCount { get; set; } public
ArticlesDTO
articles
{ get; set; } = new(); } }