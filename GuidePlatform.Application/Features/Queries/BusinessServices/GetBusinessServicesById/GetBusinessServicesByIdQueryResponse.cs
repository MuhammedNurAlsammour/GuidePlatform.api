using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos;	
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;

namespace GuidePlatform.Application.Features.Queries.BusinessServices.GetBusinessServicesById
{
    public class GetBusinessServicesByIdQueryResponse
    {
        public int TotalCount { get; set; }
        public BusinessServicesDTO businessServices { get; set; } = new();
    }
}
