using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Banners.GetAllDropboxesBanners
{
	public class GetAllDropboxesBannersQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBannersQueryResponse>>
	{
	}
}
