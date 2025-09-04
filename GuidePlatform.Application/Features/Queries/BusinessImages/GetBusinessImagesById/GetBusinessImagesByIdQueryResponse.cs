using E = GuidePlatform.Domain.Entities;
using GuidePlatform.Application.Dtos.ResponseDtos;	
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetBusinessImagesById
{
    public class GetBusinessImagesByIdQueryResponse : BaseResponseDTO
    {
        public int TotalCount { get; set; }
        public BusinessImagesDTO businessImages { get; set; } = new();
    }
}
