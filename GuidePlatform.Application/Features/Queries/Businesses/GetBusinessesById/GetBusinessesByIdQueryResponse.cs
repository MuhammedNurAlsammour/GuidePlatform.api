using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Dtos.Base;

namespace GuidePlatform.Application.Features.Queries.Businesses.GetBusinessesById
{
    public class GetBusinessesByIdQueryResponse : BaseResponseDto
    {
        public int TotalCount { get; set; }
        public BusinessesDTO businesses { get; set; } = new();
    }
}
