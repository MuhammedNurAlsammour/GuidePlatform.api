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
    /// Admin Ana Ekran SearchLogs Arama Kayıtları Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm SearchLogs Arama Kayıtlarılerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm SearchLogs Arama Kayıtlarıleri getirme parametrelerini içeren istek.</param>
    /// <returns>SearchLogs Arama Kayıtları listesini döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "SearchLogs Arama Kayıtları Listesi Getirir", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<GetAllSearchLogsQueryResponse>>> GetAllSearchLogs([FromQuery] GetAllSearchLogsQueryRequest request)
    {
      return await SendQuery<GetAllSearchLogsQueryRequest, GetAllSearchLogsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre SearchLogs Arama Kayıtları bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir SearchLogs Arama Kayıtları kimliğine göre SearchLogs Arama Kayıtları bilgilerini getirir.
    /// </remarks>
    /// <param name="request">SearchLogs Arama Kayıtları kimliğini içeren istek.</param>
    /// <returns>SearchLogs Arama Kayıtları bilgilerini döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">SearchLogs Arama Kayıtları bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre SearchLogs Arama Kayıtları Bilgilerini Görüntüle", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<GetSearchLogsByIdQueryResponse>>> GetByIdSearchLogs([FromQuery] GetSearchLogsByIdQueryRequest request)
    {
      return await SendQuery<GetSearchLogsByIdQueryRequest, GetSearchLogsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes SearchLogs Arama Kayıtları bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes SearchLogs Arama Kayıtları bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes SearchLogs Arama Kayıtları bilgilerini içeren istek.</param> 
    /// <returns>SearchLogs Arama Kayıtları bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes SearchLogs Arama Kayıtları bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">SearchLogs Arama Kayıtları bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes SearchLogs Arama Kayıtları Bilgilerini Görüntüle", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesSearchLogsQueryResponse>>> GetAllDropboxesSearchLogs([FromQuery] GetAllDropboxesSearchLogsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesSearchLogsQueryRequest, GetAllDropboxesSearchLogsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir SearchLogs Arama Kayıtları ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir SearchLogs Arama Kayıtları ekler.
    /// </remarks>
    /// <param name="request">Yeni SearchLogs Arama Kayıtları bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">SearchLogs Arama Kayıtları başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "SearchLogs Arama Kayıtları Eklemek", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<CreateSearchLogsCommandResponse>>> CreateSearchLogs([FromBody] CreateSearchLogsCommandRequest request)
    {
      return await SendCommand<CreateSearchLogsCommandRequest, CreateSearchLogsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir SearchLogs Arama Kayıtları kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip SearchLogs Arama Kayıtlarınin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek SearchLogs Arama Kayıtları bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek SearchLogs Arama Kayıtları bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "SearchLogs Arama Kayıtları Güncelemek", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<UpdateSearchLogsCommandResponse>>> UpdateSearchLogs([FromBody] UpdateSearchLogsCommandRequest request)
    {
      return await SendCommand<UpdateSearchLogsCommandRequest, UpdateSearchLogsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip SearchLogs Arama Kayıtları kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip SearchLogs Arama Kayıtları kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek SearchLogs Arama Kayıtları kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">SearchLogs Arama Kayıtları başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek SearchLogs Arama Kayıtları bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "SearchLogs Arama Kayıtları Silme", Menu = "SearchLogs-Arama Kayıtları")]
    public async Task<ActionResult<TransactionResultPack<DeleteSearchLogsCommandResponse>>> DeleteSearchLogs([FromRoute] DeleteSearchLogsCommandRequest request)
    {
      return await SendCommand<DeleteSearchLogsCommandRequest, DeleteSearchLogsCommandResponse>(request);
    }
  }
}
