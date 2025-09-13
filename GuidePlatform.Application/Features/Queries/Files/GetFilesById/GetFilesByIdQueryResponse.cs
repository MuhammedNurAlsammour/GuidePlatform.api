using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Files; namespace
GuidePlatform.Application.Features.Queries.Files.GetFilesById
{ public class
GetFilesByIdQueryResponse { public int TotalCount { get; set; } public
FilesDTO
files
{ get; set; } = new(); } }