using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Subscriptions;
using GuidePlatform.Application.Features.Commands.Subscriptions.CreateSubscriptions;
using GuidePlatform.Application.Features.Commands.Subscriptions.DeleteSubscriptions;
using GuidePlatform.Application.Features.Commands.Subscriptions.UpdateSubscriptions;
using GuidePlatform.Application.Features.Queries.Subscriptions.GetAllSubscriptions;
using GuidePlatform.Application.Features.Queries.Subscriptions.GetSubscriptionsById;
using GuidePlatform.Application.Features.Queries.Subscriptions.GetAllDropboxesSubscriptions;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class SubscriptionsController : BaseController
  {
    public SubscriptionsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Subscriptions Abonelikler tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Subscriptions Abonelikler tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Subscriptions Abonelikler tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Subscriptions Abonelikler tablosu listesini döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Subscriptions Abonelikler tablosu Listesi Getirir", Menu = "Subscriptions Abonelikler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllSubscriptionsQueryResponse>>> GetAllSubscriptions([FromQuery] GetAllSubscriptionsQueryRequest request)
    {
      return await SendQuery<GetAllSubscriptionsQueryRequest, GetAllSubscriptionsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Subscriptions Abonelikler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Subscriptions Abonelikler tablosu kimliğine göre Subscriptions Abonelikler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Subscriptions Abonelikler tablosu kimliğini içeren istek.</param>
    /// <returns>Subscriptions Abonelikler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Subscriptions Abonelikler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Subscriptions Abonelikler tablosu Bilgilerini Görüntüle", Menu = "Subscriptions Abonelikler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetSubscriptionsByIdQueryResponse>>> GetByIdSubscriptions([FromQuery] GetSubscriptionsByIdQueryRequest request)
    {
      return await SendQuery<GetSubscriptionsByIdQueryRequest, GetSubscriptionsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Subscriptions Abonelikler tablosu tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Subscriptions Abonelikler tablosu tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Subscriptions Abonelikler tablosu tablosu bilgilerini içeren istek.</param> 
    /// <returns>Subscriptions Abonelikler tablosu tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Subscriptions Abonelikler tablosu tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Subscriptions Abonelikler tablosu tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Subscriptions Abonelikler tablosu tablosu Bilgilerini Görüntüle", Menu = "Subscriptions Abonelikler tablosu tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesSubscriptionsQueryResponse>>> GetAllDropboxesSubscriptions([FromQuery] GetAllDropboxesSubscriptionsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesSubscriptionsQueryRequest, GetAllDropboxesSubscriptionsQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir Subscriptions Abonelikler tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Subscriptions Abonelikler tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Subscriptions Abonelikler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Subscriptions Abonelikler tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Subscriptions Abonelikler tablosu Eklemek", Menu = "Subscriptions Abonelikler tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateSubscriptionsCommandResponse>>> CreateSubscriptions([FromBody] CreateSubscriptionsCommandRequest request)
    {
      return await SendCommand<CreateSubscriptionsCommandRequest, CreateSubscriptionsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Subscriptions Abonelikler tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Subscriptions Abonelikler tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Subscriptions Abonelikler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Subscriptions Abonelikler tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Subscriptions Abonelikler tablosu Güncelemek", Menu = "Subscriptions Abonelikler tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateSubscriptionsCommandResponse>>> UpdateSubscriptions([FromBody] UpdateSubscriptionsCommandRequest request)
    {
      return await SendCommand<UpdateSubscriptionsCommandRequest, UpdateSubscriptionsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Subscriptions Abonelikler tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Subscriptions Abonelikler tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Subscriptions Abonelikler tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Subscriptions Abonelikler tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Subscriptions Abonelikler tablosu Silme", Menu = "Subscriptions Abonelikler tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteSubscriptionsCommandResponse>>> DeleteSubscriptions([FromRoute] DeleteSubscriptionsCommandRequest request)
    {
      return await SendCommand<DeleteSubscriptionsCommandRequest, DeleteSubscriptionsCommandResponse>(request);
    }
  }
}
