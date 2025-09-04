using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessServices;
using GuidePlatform.Application.Features.Commands.BusinessServices.CreateBusinessServices;
using GuidePlatform.Application.Features.Commands.BusinessServices.DeleteBusinessServices;
using GuidePlatform.Application.Features.Commands.BusinessServices.UpdateBusinessServices;
using GuidePlatform.Application.Features.Queries.BusinessServices.GetAllBusinessServices;
using GuidePlatform.Application.Features.Queries.BusinessServices.GetBusinessServicesById;
using GuidePlatform.Application.Features.Queries.BusinessServices.GetAllDropboxesBusinessServices;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessServicesController : BaseController
  {
    public BusinessServicesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran BusinessServices İşletme Hizmetleri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessServices İşletme Hizmetleri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessServices İşletme Hizmetleri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessServices İşletme Hizmetleri tablosu listesini döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessServices İşletme Hizmetleri tablosu Listesi Getirir", Menu = "BusinessServices İşletme Hizmetleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessServicesQueryResponse>>> GetAllBusinessServices([FromQuery] GetAllBusinessServicesQueryRequest request)
    {
      return await SendQuery<GetAllBusinessServicesQueryRequest, GetAllBusinessServicesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessServices İşletme Hizmetleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessServices İşletme Hizmetleri tablosu kimliğine göre BusinessServices İşletme Hizmetleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessServices İşletme Hizmetleri tablosu kimliğini içeren istek.</param>
    /// <returns>BusinessServices İşletme Hizmetleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessServices İşletme Hizmetleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessServices İşletme Hizmetleri tablosu Bilgilerini Görüntüle", Menu = "BusinessServices İşletme Hizmetleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessServicesByIdQueryResponse>>> GetByIdBusinessServices([FromQuery] GetBusinessServicesByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessServicesByIdQueryRequest, GetBusinessServicesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessServices İşletme Hizmetleri tablosu tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessServices İşletme Hizmetleri tablosu tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessServices İşletme Hizmetleri tablosu tablosu bilgilerini içeren istek.</param> 
    /// <returns>BusinessServices İşletme Hizmetleri tablosu tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessServices İşletme Hizmetleri tablosu tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessServices İşletme Hizmetleri tablosu tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessServices İşletme Hizmetleri tablosu tablosu Bilgilerini Görüntüle", Menu = "BusinessServices İşletme Hizmetleri tablosu tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessServicesQueryResponse>>> GetAllDropboxesBusinessServices([FromQuery] GetAllDropboxesBusinessServicesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessServicesQueryRequest, GetAllDropboxesBusinessServicesQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir BusinessServices İşletme Hizmetleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessServices İşletme Hizmetleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessServices İşletme Hizmetleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessServices İşletme Hizmetleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessServices İşletme Hizmetleri tablosu Eklemek", Menu = "BusinessServices İşletme Hizmetleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessServicesCommandResponse>>> CreateBusinessServices([FromBody] CreateBusinessServicesCommandRequest request)
    {
      return await SendCommand<CreateBusinessServicesCommandRequest, CreateBusinessServicesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessServices İşletme Hizmetleri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessServices İşletme Hizmetleri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessServices İşletme Hizmetleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessServices İşletme Hizmetleri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessServices İşletme Hizmetleri tablosu Güncelemek", Menu = "BusinessServices İşletme Hizmetleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessServicesCommandResponse>>> UpdateBusinessServices([FromBody] UpdateBusinessServicesCommandRequest request)
    {
      return await SendCommand<UpdateBusinessServicesCommandRequest, UpdateBusinessServicesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessServices İşletme Hizmetleri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessServices İşletme Hizmetleri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessServices İşletme Hizmetleri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessServices İşletme Hizmetleri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessServices İşletme Hizmetleri tablosu Silme", Menu = "BusinessServices İşletme Hizmetleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessServicesCommandResponse>>> DeleteBusinessServices([FromRoute] DeleteBusinessServicesCommandRequest request)
    {
      return await SendCommand<DeleteBusinessServicesCommandRequest, DeleteBusinessServicesCommandResponse>(request);
    }
  }
}
