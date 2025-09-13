using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.SearchLogs;
using GuidePlatform.Application.Features.Commands.SearchLogs.CreateSearchLogs;
using GuidePlatform.Application.Features.Commands.SearchLogs.DeleteSearchLogs;
using GuidePlatform.Application.Features.Commands.SearchLogs.UpdateSearchLogs;
using GuidePlatform.Application.Features.Queries.SearchLogs.GetAllSearchLogs;
using GuidePlatform.Application.Features.Queries.SearchLogs.GetSearchLogsById;
using GuidePlatform.Application.Features.Queries.SearchLogs.GetAllDropboxesSearchLogs;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class SearchLogsController : BaseController
  {
    public SearchLogsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran SearchLogs Arama Kayıtları tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm SearchLogs Arama Kayıtları tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm SearchLogs Arama Kayıtları tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>SearchLogs Arama Kayıtları tablosu listesini döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "SearchLogs Arama Kayıtları tablosu Listesi Getirir", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllSearchLogsQueryResponse>>> GetAllSearchLogs([FromQuery] GetAllSearchLogsQueryRequest request)
    {
      return await SendQuery<GetAllSearchLogsQueryRequest, GetAllSearchLogsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre SearchLogs Arama Kayıtları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir SearchLogs Arama Kayıtları tablosu kimliğine göre SearchLogs Arama Kayıtları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">SearchLogs Arama Kayıtları tablosu kimliğini içeren istek.</param>
    /// <returns>SearchLogs Arama Kayıtları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">SearchLogs Arama Kayıtları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre SearchLogs Arama Kayıtları tablosu Bilgilerini Görüntüle", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetSearchLogsByIdQueryResponse>>> GetByIdSearchLogs([FromQuery] GetSearchLogsByIdQueryRequest request)
    {
      return await SendQuery<GetSearchLogsByIdQueryRequest, GetSearchLogsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes SearchLogs Arama Kayıtları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes SearchLogs Arama Kayıtları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes SearchLogs Arama Kayıtları tablosu bilgilerini içeren istek.</param> 
    /// <returns>SearchLogs Arama Kayıtları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes SearchLogs Arama Kayıtları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">SearchLogs Arama Kayıtları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes SearchLogs Arama Kayıtları tablosu Bilgilerini Görüntüle", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesSearchLogsQueryResponse>>> GetAllDropboxesSearchLogs([FromQuery] GetAllDropboxesSearchLogsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesSearchLogsQueryRequest, GetAllDropboxesSearchLogsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir SearchLogs Arama Kayıtları tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir SearchLogs Arama Kayıtları tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni SearchLogs Arama Kayıtları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">SearchLogs Arama Kayıtları tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "SearchLogs Arama Kayıtları tablosu Eklemek", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateSearchLogsCommandResponse>>> CreateSearchLogs([FromBody] CreateSearchLogsCommandRequest request)
    {
      return await SendCommand<CreateSearchLogsCommandRequest, CreateSearchLogsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir SearchLogs Arama Kayıtları tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip SearchLogs Arama Kayıtları tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek SearchLogs Arama Kayıtları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek SearchLogs Arama Kayıtları tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "SearchLogs Arama Kayıtları tablosu Güncelemek", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateSearchLogsCommandResponse>>> UpdateSearchLogs([FromBody] UpdateSearchLogsCommandRequest request)
    {
      return await SendCommand<UpdateSearchLogsCommandRequest, UpdateSearchLogsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip SearchLogs Arama Kayıtları tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip SearchLogs Arama Kayıtları tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek SearchLogs Arama Kayıtları tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek SearchLogs Arama Kayıtları tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "SearchLogs Arama Kayıtları tablosu Silme", Menu = "SearchLogs Arama Kayıtları tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteSearchLogsCommandResponse>>> DeleteSearchLogs([FromRoute] DeleteSearchLogsCommandRequest request)
    {
      return await SendCommand<DeleteSearchLogsCommandRequest, DeleteSearchLogsCommandResponse>(request);
    }
  }
}
