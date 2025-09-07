using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Notifications.GetAllDropboxesNotifications
{
	public class GetAllDropboxesNotificationsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesNotificationsQueryResponse>>
	{
	}
}
