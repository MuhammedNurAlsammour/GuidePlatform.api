using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Banners.GetBannersById
{
  public class GetBannersByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetBannersByIdQueryResponse>>
  {
    public Guid? ProvinceId { get; set; } // Belirli bir şehre ait arayanlar

	}
}
