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
    /// Admin Ana Ekran BusinessServices İşletme Hizmetleri Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessServices İşletme Hizmetlerilerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessServices İşletme Hizmetlerileri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessServices İşletme Hizmetleri listesini döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessServices İşletme Hizmetleri Listesi Getirir", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessServicesQueryResponse>>> GetAllBusinessServices([FromQuery] GetAllBusinessServicesQueryRequest request)
    {
      return await SendQuery<GetAllBusinessServicesQueryRequest, GetAllBusinessServicesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessServices İşletme Hizmetleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessServices İşletme Hizmetleri kimliğine göre BusinessServices İşletme Hizmetleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessServices İşletme Hizmetleri kimliğini içeren istek.</param>
    /// <returns>BusinessServices İşletme Hizmetleri bilgilerini döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessServices İşletme Hizmetleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessServices İşletme Hizmetleri Bilgilerini Görüntüle", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessServicesByIdQueryResponse>>> GetByIdBusinessServices([FromQuery] GetBusinessServicesByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessServicesByIdQueryRequest, GetBusinessServicesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessServices İşletme Hizmetleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessServices İşletme Hizmetleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessServices İşletme Hizmetleri bilgilerini içeren istek.</param> 
    /// <returns>BusinessServices İşletme Hizmetleri bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessServices İşletme Hizmetleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessServices İşletme Hizmetleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessServices İşletme Hizmetleri Bilgilerini Görüntüle", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessServicesQueryResponse>>> GetAllDropboxesBusinessServices([FromQuery] GetAllDropboxesBusinessServicesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessServicesQueryRequest, GetAllDropboxesBusinessServicesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessServices İşletme Hizmetleri ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessServices İşletme Hizmetleri ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessServices İşletme Hizmetleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessServices İşletme Hizmetleri başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessServices İşletme Hizmetleri Eklemek", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessServicesCommandResponse>>> CreateBusinessServices([FromBody] CreateBusinessServicesCommandRequest request)
    {
      return await SendCommand<CreateBusinessServicesCommandRequest, CreateBusinessServicesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessServices İşletme Hizmetleri kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessServices İşletme Hizmetlerinin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessServices İşletme Hizmetleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessServices İşletme Hizmetleri bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessServices İşletme Hizmetleri Güncelemek", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessServicesCommandResponse>>> UpdateBusinessServices([FromBody] UpdateBusinessServicesCommandRequest request)
    {
      return await SendCommand<UpdateBusinessServicesCommandRequest, UpdateBusinessServicesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessServices İşletme Hizmetleri kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessServices İşletme Hizmetleri kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessServices İşletme Hizmetleri kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessServices İşletme Hizmetleri başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessServices İşletme Hizmetleri bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessServices İşletme Hizmetleri Silme", Menu = "BusinessServices-İşletme Hizmetleri")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessServicesCommandResponse>>> DeleteBusinessServices([FromRoute] DeleteBusinessServicesCommandRequest request)
    {
      return await SendCommand<DeleteBusinessServicesCommandRequest, DeleteBusinessServicesCommandResponse>(request);
    }
  }
}
