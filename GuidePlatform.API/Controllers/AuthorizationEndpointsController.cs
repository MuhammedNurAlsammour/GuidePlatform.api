using GuidePlatform.API.Controllers.Base;
using GuidePlatform.Application.Abstractions.Services;
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
	public class AuthorizationEndpointsController : BaseController
	{
		private readonly IAuthorizationEndpointService _authorizationEndpointService;

		public AuthorizationEndpointsController(IMediator mediator, IAuthorizationEndpointService authorizationEndpointService) : base(mediator)
		{
			_authorizationEndpointService = authorizationEndpointService;
		}

		[HttpPost("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Role Yetki Ata", Menu = "AuthorizationEndpoints")]
		public async Task<IActionResult> AssignRoleEndpoint2(AssignEndpointsToRoleDTO request)
		{
			await _authorizationEndpointService.AssignEndpointsToRoleAsync(request, typeof(Program));
			return Ok();
		}

		[HttpPost("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Role Yetki Güncelle", Menu = "AuthorizationEndpoints")]
		public async Task<IActionResult> UpdateRoleEndpoint(AssignEndpointsToRoleDTO request)
		{
			var result = await _authorizationEndpointService.UpdateEndpointsToRoleAsync(request, typeof(Program));
			if (result)
			{
				return Ok(new { message = "Role yetkileri başarıyla güncellendi." });
			}
			return BadRequest(new { message = "Role yetkileri güncellenirken bir hata oluştu." });
		}
	}

	public class AssignRoleToEndpointReq2
	{
		public string Role { get; set; }
		public string[] Codes { get; set; }
		public string Menu { get; set; }
	}
}
