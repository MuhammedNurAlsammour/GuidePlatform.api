using GuidePlatform.API.Controllers.Base;
using GuidePlatform.Application.Features.Commands.Send.SendWpMessage;
using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GuidePlatform.API.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = "Admin")]
	public class SendWpMessageController : BaseController
	{
		public SendWpMessageController(IMediator mediator) : base(mediator)
		{
		}

		/// <summary>
		/// Yeni bir personel oluşturur.
		/// </summary>
		/// <remarks>
		/// Bu uç nokta, yeni bir personel ekler.
		/// </remarks>
		/// <param name="request">Yeni personel bilgilerini içeren istek.</param>
		/// <returns>İşlem sonucunu döndürür.</returns>
		/// <response code="201">Personel başarıyla oluşturuldu.</response>
		/// <response code="400">İstek geçersizse.</response>
		/// <response code="401">Kullanıcı yetkili değilse.</response>
		[HttpPost("[action]")]
		public async Task<ActionResult<string>> Send([FromBody] SendWpMessageCommandRequest request)
		{
			return Ok(await _mediator.Send(request));
		}
	}
}
