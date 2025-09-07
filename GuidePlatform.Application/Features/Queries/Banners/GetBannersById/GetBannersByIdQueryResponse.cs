using E =
GuidePlatform.Domain.Entities; using
GuidePlatform.Application.Dtos.ResponseDtos; using
GuidePlatform.Application.Dtos.ResponseDtos.Banners; namespace
GuidePlatform.Application.Features.Queries.Banners.GetBannersById
{ public class
GetBannersByIdQueryResponse { public int TotalCount { get; set; } public
BannersDTO
banners
{ get; set; } = new(); } }