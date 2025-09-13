using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Files.GetAllDropboxesFiles
{
	public class GetAllDropboxesFilesQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesFilesQueryResponse>>
	{
	}
}
