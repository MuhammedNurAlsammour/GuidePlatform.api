using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours; namespace
GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetBusinessWorkingHoursById
{ public class
GetBusinessWorkingHoursByIdQueryResponse { public int TotalCount { get; set; } public
BusinessWorkingHoursDTO
businessWorkingHours
{ get; set; } = new(); } }