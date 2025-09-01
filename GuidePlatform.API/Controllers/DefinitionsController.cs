using GuidePlatform.API.Controllers.Base;
using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuidePlatform.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Admin")]
	public class DefinitionsController : BaseController
	{
		public DefinitionsController(IMediator mediator) : base(mediator)
		{
		}

		/// <summary>
		/// Tanımlar sayfası  için  tanımları yetkiye göre getirir.
		/// </summary>
		/// <remarks>
		/// Bu HTTP GET Tanımlar sayfası  için tanımları yetkiye göre getirir.
		/// </remarks>
		/// <returns>HTTP 200 OK başarılı yetkilendirme durumunda döner</returns>
		/// <response code="200">Yetkilendirme başarılı</response>
		/// <response code="401"> yetkili değilse</response>
		[HttpGet("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Reading,
				  Definition = "Tanımlar Sayfası Tanımları Getirme",
				  Menu = "Tanımlar")]
		public async Task<IActionResult> GetAllDefinitions()
		{
			return Ok();
		}

		/// <summary>
		/// Tanımlar sayfası  için  kullanıcı  tanımları yetkiye göre getirir.
		/// </summary>
		/// <remarks>
		/// Bu HTTP GET Tanımlar sayfası  için  kullanıcı  tanımları yetkiye göre getirir.
		/// </remarks>
		/// <returns>HTTP 200 OK başarılı yetkilendirme durumunda döner</returns>
		/// <response code="200">Yetkilendirme başarılı</response>
		/// <response code="401">Kullanıcı yetkili değilse</response>
		[HttpGet("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Reading,
				  Definition = "Tanımlar Sayfası Kullanıcı Tanımları Getirme",
				  Menu = "Tanımlar")]
		public async Task<IActionResult> GetAllUser()
		{
			return Ok();
		}

		/// <summary>
		/// Tanımlar sayfası  için  rol  tanımları yetkiye göre getirir.
		/// </summary>
		/// <remarks>
		/// Bu HTTP GET Tanımlar sayfası  için  rol  tanımları yetkiye göre getirir.
		/// </remarks>
		/// <returns>HTTP 200 OK başarılı yetkilendirme durumunda döner</returns>
		/// <response code="200">Yetkilendirme başarılı</response>
		/// <response code="401">Kullanıcı yetkili değilse</response>
		[HttpGet("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Reading,
				  Definition = "Tanımlar Sayfası Rol Tanımları Getirme",
				  Menu = "Tanımlar")]
		public async Task<IActionResult> GetAllRoles()
		{
			return Ok();
		}

		/// <summary>
		/// Tanımlar sayfası  için  müşteri  tanımları yetkiye göre getirir.
		/// </summary>
		/// <remarks>
		/// Bu HTTP GET Tanımlar sayfası  için  müşteri  tanımları yetkiye göre getirir.
		/// </remarks>
		/// <returns>HTTP 200 OK başarılı yetkilendirme durumunda döner</returns>
		/// <response code="200">Yetkilendirme başarılı</response>
		/// <response code="401">Kullanıcı yetkili değilse</response>
		[HttpGet("[action]")]
		[Authorize(AuthenticationSchemes = "Admin")]
		[AuthorizeDefinition(ActionType = ActionType.Reading,
				  Definition = "Tanımlar Sayfası Müşteri Tanımları Getirme",
				  Menu = "Tanımlar")]
		public async Task<IActionResult> GetAllCustomers()
		{
			return Ok();
		}
	}
}
