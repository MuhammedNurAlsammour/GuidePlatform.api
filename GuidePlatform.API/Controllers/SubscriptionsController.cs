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
    /// Admin Ana Ekran Subscriptions Abonelikler Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Subscriptions Aboneliklerlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Subscriptions Aboneliklerleri getirme parametrelerini içeren istek.</param>
    /// <returns>Subscriptions Abonelikler listesini döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Subscriptions Abonelikler Listesi Getirir", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<GetAllSubscriptionsQueryResponse>>> GetAllSubscriptions([FromQuery] GetAllSubscriptionsQueryRequest request)
    {
      return await SendQuery<GetAllSubscriptionsQueryRequest, GetAllSubscriptionsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Subscriptions Abonelikler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Subscriptions Abonelikler kimliğine göre Subscriptions Abonelikler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Subscriptions Abonelikler kimliğini içeren istek.</param>
    /// <returns>Subscriptions Abonelikler bilgilerini döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Subscriptions Abonelikler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Subscriptions Abonelikler Bilgilerini Görüntüle", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<GetSubscriptionsByIdQueryResponse>>> GetByIdSubscriptions([FromQuery] GetSubscriptionsByIdQueryRequest request)
    {
      return await SendQuery<GetSubscriptionsByIdQueryRequest, GetSubscriptionsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Subscriptions Abonelikler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Subscriptions Abonelikler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Subscriptions Abonelikler bilgilerini içeren istek.</param> 
    /// <returns>Subscriptions Abonelikler bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Subscriptions Abonelikler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Subscriptions Abonelikler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Subscriptions Abonelikler Bilgilerini Görüntüle", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesSubscriptionsQueryResponse>>> GetAllDropboxesSubscriptions([FromQuery] GetAllDropboxesSubscriptionsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesSubscriptionsQueryRequest, GetAllDropboxesSubscriptionsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Subscriptions Abonelikler ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Subscriptions Abonelikler ekler.
    /// </remarks>
    /// <param name="request">Yeni Subscriptions Abonelikler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Subscriptions Abonelikler başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Subscriptions Abonelikler Eklemek", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<CreateSubscriptionsCommandResponse>>> CreateSubscriptions([FromBody] CreateSubscriptionsCommandRequest request)
    {
      return await SendCommand<CreateSubscriptionsCommandRequest, CreateSubscriptionsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Subscriptions Abonelikler kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Subscriptions Aboneliklernin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Subscriptions Abonelikler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Subscriptions Abonelikler bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Subscriptions Abonelikler Güncelemek", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<UpdateSubscriptionsCommandResponse>>> UpdateSubscriptions([FromBody] UpdateSubscriptionsCommandRequest request)
    {
      return await SendCommand<UpdateSubscriptionsCommandRequest, UpdateSubscriptionsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Subscriptions Abonelikler kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Subscriptions Abonelikler kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Subscriptions Abonelikler kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Subscriptions Abonelikler başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Subscriptions Abonelikler bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Subscriptions Abonelikler Silme", Menu = "Subscriptions-Abonelikler")]
    public async Task<ActionResult<TransactionResultPack<DeleteSubscriptionsCommandResponse>>> DeleteSubscriptions([FromRoute] DeleteSubscriptionsCommandRequest request)
    {
      return await SendCommand<DeleteSubscriptionsCommandRequest, DeleteSubscriptionsCommandResponse>(request);
    }
  }
}
