using GuidePlatform.Application.Dtos.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GuidePlatform.API.Controllers.Base
{
	/// <summary>
	/// Base class for all controllers with common functionality
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected readonly IMediator _mediator;

		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}

		/// <summary>
		/// Send command and return result with proper status code
		/// </summary>
		protected async Task<ActionResult<TransactionResultPack<TResponse>>> SendCommand<TRequest, TResponse>(TRequest request)
			where TRequest : MediatR.IRequest<TransactionResultPack<TResponse>>
		{
			var response = await _mediator.Send(request);

			if (response.OperationStatus)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		/// <summary>
		/// Send command and return result with custom status code
		/// </summary>
		protected async Task<ActionResult<TransactionResultPack<TResponse>>> SendCommand<TRequest, TResponse>(TRequest request, HttpStatusCode successStatusCode)
			where TRequest : MediatR.IRequest<TransactionResultPack<TResponse>>
		{
			var response = await _mediator.Send(request);

			if (response.OperationStatus)
			{
				return StatusCode((int)successStatusCode, response);
			}

			return BadRequest(response);
		}

		/// <summary>
		/// Send query and return result
		/// </summary>
		protected async Task<ActionResult<TransactionResultPack<TResponse>>> SendQuery<TRequest, TResponse>(TRequest request)
			where TRequest : MediatR.IRequest<TransactionResultPack<TResponse>>
		{
			var response = await _mediator.Send(request);

			if (response.OperationStatus)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}
	}
}
