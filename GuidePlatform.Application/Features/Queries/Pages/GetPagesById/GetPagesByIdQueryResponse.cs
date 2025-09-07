using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Pages; namespace
GuidePlatform.Application.Features.Queries.Pages.GetPagesById
{ public class
GetPagesByIdQueryResponse { public int TotalCount { get; set; } public
PagesDTO
pages
{ get; set; } = new(); } }