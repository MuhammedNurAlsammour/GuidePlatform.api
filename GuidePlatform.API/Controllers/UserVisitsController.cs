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
    /// Admin Ana Ekran UserVisits Kullanıcı Ziyaretleri Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm UserVisits Kullanıcı Ziyaretlerilerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm UserVisits Kullanıcı Ziyaretlerileri getirme parametrelerini içeren istek.</param>
    /// <returns>UserVisits Kullanıcı Ziyaretleri listesini döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "UserVisits Kullanıcı Ziyaretleri Listesi Getirir", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllUserVisitsQueryResponse>>> GetAllUserVisits([FromQuery] GetAllUserVisitsQueryRequest request)
    {
      return await SendQuery<GetAllUserVisitsQueryRequest, GetAllUserVisitsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre UserVisits Kullanıcı Ziyaretleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir UserVisits Kullanıcı Ziyaretleri kimliğine göre UserVisits Kullanıcı Ziyaretleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">UserVisits Kullanıcı Ziyaretleri kimliğini içeren istek.</param>
    /// <returns>UserVisits Kullanıcı Ziyaretleri bilgilerini döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserVisits Kullanıcı Ziyaretleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre UserVisits Kullanıcı Ziyaretleri Bilgilerini Görüntüle", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<GetUserVisitsByIdQueryResponse>>> GetByIdUserVisits([FromQuery] GetUserVisitsByIdQueryRequest request)
    {
      return await SendQuery<GetUserVisitsByIdQueryRequest, GetUserVisitsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes UserVisits Kullanıcı Ziyaretleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes UserVisits Kullanıcı Ziyaretleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes UserVisits Kullanıcı Ziyaretleri bilgilerini içeren istek.</param> 
    /// <returns>UserVisits Kullanıcı Ziyaretleri bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes UserVisits Kullanıcı Ziyaretleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserVisits Kullanıcı Ziyaretleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes UserVisits Kullanıcı Ziyaretleri Bilgilerini Görüntüle", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesUserVisitsQueryResponse>>> GetAllDropboxesUserVisits([FromQuery] GetAllDropboxesUserVisitsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesUserVisitsQueryRequest, GetAllDropboxesUserVisitsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir UserVisits Kullanıcı Ziyaretleri ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir UserVisits Kullanıcı Ziyaretleri ekler.
    /// </remarks>
    /// <param name="request">Yeni UserVisits Kullanıcı Ziyaretleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">UserVisits Kullanıcı Ziyaretleri başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "UserVisits Kullanıcı Ziyaretleri Eklemek", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<CreateUserVisitsCommandResponse>>> CreateUserVisits([FromBody] CreateUserVisitsCommandRequest request)
    {
      return await SendCommand<CreateUserVisitsCommandRequest, CreateUserVisitsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir UserVisits Kullanıcı Ziyaretleri kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretlerinin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek UserVisits Kullanıcı Ziyaretleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek UserVisits Kullanıcı Ziyaretleri bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "UserVisits Kullanıcı Ziyaretleri Güncelemek", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<UpdateUserVisitsCommandResponse>>> UpdateUserVisits([FromBody] UpdateUserVisitsCommandRequest request)
    {
      return await SendCommand<UpdateUserVisitsCommandRequest, UpdateUserVisitsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretleri kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserVisits Kullanıcı Ziyaretleri kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek UserVisits Kullanıcı Ziyaretleri kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserVisits Kullanıcı Ziyaretleri başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek UserVisits Kullanıcı Ziyaretleri bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "UserVisits Kullanıcı Ziyaretleri Silme", Menu = "UserVisits-Kullanıcı Ziyaretleri")]
    public async Task<ActionResult<TransactionResultPack<DeleteUserVisitsCommandResponse>>> DeleteUserVisits([FromRoute] DeleteUserVisitsCommandRequest request)
    {
      return await SendCommand<DeleteUserVisitsCommandRequest, DeleteUserVisitsCommandResponse>(request);
    }
  }
}
