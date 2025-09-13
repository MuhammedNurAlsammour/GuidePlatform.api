using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.UserVisits;
using GuidePlatform.Application.Features.Commands.UserVisits.CreateUserVisits;
using GuidePlatform.Application.Features.Commands.UserVisits.DeleteUserVisits;
using GuidePlatform.Application.Features.Commands.UserVisits.UpdateUserVisits;
using GuidePlatform.Application.Features.Queries.UserVisits.GetAllUserVisits;
using GuidePlatform.Application.Features.Queries.UserVisits.GetUserVisitsById;
using GuidePlatform.Application.Features.Queries.UserVisits.GetAllDropboxesUserVisits;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class UserVisitsController : BaseController
  {
    public UserVisitsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran UserVisits Kullanıcı Ziyaretleri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm UserVisits Kullanıcı Ziyaretleri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm UserVisits Kullanıcı Ziyaretleri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>UserVisits Kullanıcı Ziyaretleri tablosu listesini döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "UserVisits Kullanıcı Ziyaretleri tablosu Listesi Getirir", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllUserVisitsQueryResponse>>> GetAllUserVisits([FromQuery] GetAllUserVisitsQueryRequest request)
    {
      return await SendQuery<GetAllUserVisitsQueryRequest, GetAllUserVisitsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir UserVisits Kullanıcı Ziyaretleri tablosu kimliğine göre UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">UserVisits Kullanıcı Ziyaretleri tablosu kimliğini içeren istek.</param>
    /// <returns>UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserVisits Kullanıcı Ziyaretleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre UserVisits Kullanıcı Ziyaretleri tablosu Bilgilerini Görüntüle", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetUserVisitsByIdQueryResponse>>> GetByIdUserVisits([FromQuery] GetUserVisitsByIdQueryRequest request)
    {
      return await SendQuery<GetUserVisitsByIdQueryRequest, GetUserVisitsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini içeren istek.</param> 
    /// <returns>UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserVisits Kullanıcı Ziyaretleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes UserVisits Kullanıcı Ziyaretleri tablosu Bilgilerini Görüntüle", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesUserVisitsQueryResponse>>> GetAllDropboxesUserVisits([FromQuery] GetAllDropboxesUserVisitsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesUserVisitsQueryRequest, GetAllDropboxesUserVisitsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir UserVisits Kullanıcı Ziyaretleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir UserVisits Kullanıcı Ziyaretleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">UserVisits Kullanıcı Ziyaretleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "UserVisits Kullanıcı Ziyaretleri tablosu Eklemek", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateUserVisitsCommandResponse>>> CreateUserVisits([FromBody] CreateUserVisitsCommandRequest request)
    {
      return await SendCommand<CreateUserVisitsCommandRequest, CreateUserVisitsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir UserVisits Kullanıcı Ziyaretleri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretleri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek UserVisits Kullanıcı Ziyaretleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek UserVisits Kullanıcı Ziyaretleri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "UserVisits Kullanıcı Ziyaretleri tablosu Güncelemek", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateUserVisitsCommandResponse>>> UpdateUserVisits([FromBody] UpdateUserVisitsCommandRequest request)
    {
      return await SendCommand<UpdateUserVisitsCommandRequest, UpdateUserVisitsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretleri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretleri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek UserVisits Kullanıcı Ziyaretleri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek UserVisits Kullanıcı Ziyaretleri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "UserVisits Kullanıcı Ziyaretleri tablosu Silme", Menu = "UserVisits Kullanıcı Ziyaretleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteUserVisitsCommandResponse>>> DeleteUserVisits([FromRoute] DeleteUserVisitsCommandRequest request)
    {
      return await SendCommand<DeleteUserVisitsCommandRequest, DeleteUserVisitsCommandResponse>(request);
    }
  }
}
