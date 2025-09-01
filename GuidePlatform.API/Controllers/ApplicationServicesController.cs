using GuidePlatform.API.Controllers.Base;
using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.DTOs;
using Karmed.External.Auth.Library.Enums;
using Karmed.External.Auth.Library.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GuidePlatform.API.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = "Admin")]
	public class ApplicationServicesController : BaseController
	{
		private readonly IApplicationService _applicationService;

		public ApplicationServicesController(IMediator mediator, IApplicationService applicationService) : base(mediator)
		{
			_applicationService = applicationService;
		}

		[HttpGet("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[ProducesResponseType<List<Menu>>(StatusCodes.Status200OK)]
		[AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Authorize Definition Endpoints", Menu = "Application Services")]
		public IActionResult GetAuthorizeDefinitionEndpoints()
		{
			return Ok(_applicationService.GetAuthorizeDefinitionEndpoints(typeof(Program)));
		}
	}
}

