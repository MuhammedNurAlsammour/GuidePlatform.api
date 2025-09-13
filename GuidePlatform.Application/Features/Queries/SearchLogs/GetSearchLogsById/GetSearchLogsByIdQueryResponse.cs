using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs; namespace
GuidePlatform.Application.Features.Queries.SearchLogs.GetSearchLogsById
{ public class
GetSearchLogsByIdQueryResponse { public int TotalCount { get; set; } public
SearchLogsDTO
searchLogs
{ get; set; } = new(); } }