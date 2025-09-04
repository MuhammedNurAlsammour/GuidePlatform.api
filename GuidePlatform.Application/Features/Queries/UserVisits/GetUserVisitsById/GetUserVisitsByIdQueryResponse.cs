using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos;	
using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;

namespace GuidePlatform.Application.Features.Queries.UserVisits.GetUserVisitsById
{
    public class GetUserVisitsByIdQueryResponse : BaseResponseDTO
    {
        public int TotalCount { get; set; }
        public UserVisitsDTO userVisits { get; set; } = new();
    }
}
