using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.Announcements.GetAnnouncementsById
{
	public class GetAnnouncementsByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetAnnouncementsByIdQueryResponse>>
	{

	}
}
