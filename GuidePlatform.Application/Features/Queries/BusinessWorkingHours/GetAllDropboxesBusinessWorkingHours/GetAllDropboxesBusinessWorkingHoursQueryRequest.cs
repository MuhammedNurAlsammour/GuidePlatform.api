using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllDropboxesBusinessWorkingHours
{
	public class GetAllDropboxesBusinessWorkingHoursQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesBusinessWorkingHoursQueryResponse>>
	{
	}
}
