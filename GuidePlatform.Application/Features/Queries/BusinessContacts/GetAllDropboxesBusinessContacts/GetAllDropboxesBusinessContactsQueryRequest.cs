using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllDropboxesBusinessContacts
{
	public class GetAllDropboxesBusinessContactsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessContactsQueryResponse>>
	{
	}
}
