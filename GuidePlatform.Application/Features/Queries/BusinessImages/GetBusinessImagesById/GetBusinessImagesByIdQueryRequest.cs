using MediatR;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.Features.Queries.BusinessImages.GetBusinessImagesById
{
	public class GetBusinessImagesByIdQueryRequest : BaseQueryRequest, IRequest<TransactionResultPack<GetBusinessImagesByIdQueryResponse>>
	{

	}
}
