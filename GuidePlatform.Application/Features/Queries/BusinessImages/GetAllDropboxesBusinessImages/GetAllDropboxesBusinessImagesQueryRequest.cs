using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetAllDropboxesBusinessImages
{
	public class GetAllDropboxesBusinessImagesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessImagesQueryResponse>>
	{
	}
}
