using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessAnalytics;
using GuidePlatform.Application.Features.Commands.BusinessAnalytics.CreateBusinessAnalytics;
using GuidePlatform.Application.Features.Commands.BusinessAnalytics.DeleteBusinessAnalytics;
using GuidePlatform.Application.Features.Commands.BusinessAnalytics.UpdateBusinessAnalytics;
using GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllBusinessAnalytics;
using GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetBusinessAnalyticsById;
using GuidePlatform.Application.Features.Queries.BusinessAnalytics.GetAllDropboxesBusinessAnalytics;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessAnalyticsController : BaseController
  {
    public BusinessAnalyticsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran BusinessAnalytics İşletme Analizleri Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessAnalytics İşletme Analizlerilerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessAnalytics İşletme Analizlerileri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessAnalytics İşletme Analizleri listesini döndürür.</returns>
    /// <response code="200">BusinessAnalytics İşletme Analizleri listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessAnalytics İşletme Analizleri Listesi Getirir", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessAnalyticsQueryResponse>>> GetAllBusinessAnalytics([FromQuery] GetAllBusinessAnalyticsQueryRequest request)
    {
      return await SendQuery<GetAllBusinessAnalyticsQueryRequest, GetAllBusinessAnalyticsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessAnalytics İşletme Analizleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessAnalytics İşletme Analizleri kimliğine göre BusinessAnalytics İşletme Analizleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessAnalytics İşletme Analizleri kimliğini içeren istek.</param>
    /// <returns>BusinessAnalytics İşletme Analizleri bilgilerini döndürür.</returns>
    /// <response code="200">BusinessAnalytics İşletme Analizleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessAnalytics İşletme Analizleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessAnalytics İşletme Analizleri Bilgilerini Görüntüle", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessAnalyticsByIdQueryResponse>>> GetByIdBusinessAnalytics([FromQuery] GetBusinessAnalyticsByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessAnalyticsByIdQueryRequest, GetBusinessAnalyticsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessAnalytics İşletme Analizleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessAnalytics İşletme Analizleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessAnalytics İşletme Analizleri bilgilerini içeren istek.</param> 
    /// <returns>BusinessAnalytics İşletme Analizleri bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessAnalytics İşletme Analizleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessAnalytics İşletme Analizleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessAnalytics İşletme Analizleri Bilgilerini Görüntüle", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessAnalyticsQueryResponse>>> GetAllDropboxesBusinessAnalytics([FromQuery] GetAllDropboxesBusinessAnalyticsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessAnalyticsQueryRequest, GetAllDropboxesBusinessAnalyticsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessAnalytics İşletme Analizleri ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessAnalytics İşletme Analizleri ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessAnalytics İşletme Analizleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessAnalytics İşletme Analizleri başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessAnalytics İşletme Analizleri Eklemek", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessAnalyticsCommandResponse>>> CreateBusinessAnalytics([FromBody] CreateBusinessAnalyticsCommandRequest request)
    {
      return await SendCommand<CreateBusinessAnalyticsCommandRequest, CreateBusinessAnalyticsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessAnalytics İşletme Analizleri kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessAnalytics İşletme Analizlerinin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessAnalytics İşletme Analizleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessAnalytics İşletme Analizleri başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessAnalytics İşletme Analizleri bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessAnalytics İşletme Analizleri Güncelemek", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessAnalyticsCommandResponse>>> UpdateBusinessAnalytics([FromBody] UpdateBusinessAnalyticsCommandRequest request)
    {
      return await SendCommand<UpdateBusinessAnalyticsCommandRequest, UpdateBusinessAnalyticsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessAnalytics İşletme Analizleri kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessAnalytics İşletme Analizleri kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessAnalytics İşletme Analizleri kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessAnalytics İşletme Analizleri başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessAnalytics İşletme Analizleri bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessAnalytics İşletme Analizleri Silme", Menu = "BusinessAnalytics-İşletme Analizleri")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessAnalyticsCommandResponse>>> DeleteBusinessAnalytics([FromRoute] DeleteBusinessAnalyticsCommandRequest request)
    {
      return await SendCommand<DeleteBusinessAnalyticsCommandRequest, DeleteBusinessAnalyticsCommandResponse>(request);
    }
  }
}
