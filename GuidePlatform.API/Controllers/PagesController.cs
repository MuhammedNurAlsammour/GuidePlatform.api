using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Pages;
using GuidePlatform.Application.Features.Commands.Pages.CreatePages;
using GuidePlatform.Application.Features.Commands.Pages.DeletePages;
using GuidePlatform.Application.Features.Commands.Pages.UpdatePages;
using GuidePlatform.Application.Features.Queries.Pages.GetAllPages;
using GuidePlatform.Application.Features.Queries.Pages.GetPagesById;
using GuidePlatform.Application.Features.Queries.Pages.GetAllDropboxesPages;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class PagesController : BaseController
  {
    public PagesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Pages Sayfalar Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Pages Sayfalarlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Pages Sayfalarleri getirme parametrelerini içeren istek.</param>
    /// <returns>Pages Sayfalar listesini döndürür.</returns>
    /// <response code="200">Pages Sayfalar listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Pages Sayfalar Listesi Getirir", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<GetAllPagesQueryResponse>>> GetAllPages([FromQuery] GetAllPagesQueryRequest request)
    {
      return await SendQuery<GetAllPagesQueryRequest, GetAllPagesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Pages Sayfalar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Pages Sayfalar kimliğine göre Pages Sayfalar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Pages Sayfalar kimliğini içeren istek.</param>
    /// <returns>Pages Sayfalar bilgilerini döndürür.</returns>
    /// <response code="200">Pages Sayfalar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Pages Sayfalar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Pages Sayfalar Bilgilerini Görüntüle", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<GetPagesByIdQueryResponse>>> GetByIdPages([FromQuery] GetPagesByIdQueryRequest request)
    {
      return await SendQuery<GetPagesByIdQueryRequest, GetPagesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Pages Sayfalar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Pages Sayfalar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Pages Sayfalar bilgilerini içeren istek.</param> 
    /// <returns>Pages Sayfalar bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Pages Sayfalar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Pages Sayfalar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Pages Sayfalar Bilgilerini Görüntüle", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesPagesQueryResponse>>> GetAllDropboxesPages([FromQuery] GetAllDropboxesPagesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesPagesQueryRequest, GetAllDropboxesPagesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Pages Sayfalar ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Pages Sayfalar ekler.
    /// </remarks>
    /// <param name="request">Yeni Pages Sayfalar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Pages Sayfalar başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Pages Sayfalar Eklemek", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<CreatePagesCommandResponse>>> CreatePages([FromBody] CreatePagesCommandRequest request)
    {
      return await SendCommand<CreatePagesCommandRequest, CreatePagesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Pages Sayfalar kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Pages Sayfalarnin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Pages Sayfalar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Pages Sayfalar başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Pages Sayfalar bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Pages Sayfalar Güncelemek", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<UpdatePagesCommandResponse>>> UpdatePages([FromBody] UpdatePagesCommandRequest request)
    {
      return await SendCommand<UpdatePagesCommandRequest, UpdatePagesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Pages Sayfalar kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Pages Sayfalar kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Pages Sayfalar kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Pages Sayfalar başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Pages Sayfalar bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Pages Sayfalar Silme", Menu = "Pages-Sayfalar")]
    public async Task<ActionResult<TransactionResultPack<DeletePagesCommandResponse>>> DeletePages([FromRoute] DeletePagesCommandRequest request)
    {
      return await SendCommand<DeletePagesCommandRequest, DeletePagesCommandResponse>(request);
    }
  }
}
