using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllDropboxesNotificationSettings
{
	public class GetAllDropboxesNotificationSettingsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesNotificationSettingsQueryResponse>>
	{
	}
}
