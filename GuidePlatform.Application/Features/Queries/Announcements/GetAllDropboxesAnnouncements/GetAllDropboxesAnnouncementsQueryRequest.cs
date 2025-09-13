using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAllDropboxesAnnouncements
{
	public class GetAllDropboxesAnnouncementsQueryRequest : BasePaginatedQueryRequest, IRequest<TransactionResultPack<GetAllDropboxesAnnouncementsQueryResponse>>
	{
	}
}
