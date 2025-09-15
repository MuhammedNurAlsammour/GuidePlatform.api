using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Notifications;
using GuidePlatform.Application.Features.Commands.Notifications.CreateNotifications;
using GuidePlatform.Application.Features.Commands.Notifications.DeleteNotifications;
using GuidePlatform.Application.Features.Commands.Notifications.UpdateNotifications;
using GuidePlatform.Application.Features.Queries.Notifications.GetAllNotifications;
using GuidePlatform.Application.Features.Queries.Notifications.GetNotificationsById;
using GuidePlatform.Application.Features.Queries.Notifications.GetAllDropboxesNotifications;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class NotificationsController : BaseController
  {
    public NotificationsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Notifications Bildirimler Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Notifications Bildirimlerlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Notifications Bildirimlerleri getirme parametrelerini içeren istek.</param>
    /// <returns>Notifications Bildirimler listesini döndürür.</returns>
    /// <response code="200">Notifications Bildirimler listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Notifications Bildirimler Listesi Getirir", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<GetAllNotificationsQueryResponse>>> GetAllNotifications([FromQuery] GetAllNotificationsQueryRequest request)
    {
      return await SendQuery<GetAllNotificationsQueryRequest, GetAllNotificationsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Notifications Bildirimler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Notifications Bildirimler kimliğine göre Notifications Bildirimler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Notifications Bildirimler kimliğini içeren istek.</param>
    /// <returns>Notifications Bildirimler bilgilerini döndürür.</returns>
    /// <response code="200">Notifications Bildirimler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Notifications Bildirimler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Notifications Bildirimler Bilgilerini Görüntüle", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<GetNotificationsByIdQueryResponse>>> GetByIdNotifications([FromQuery] GetNotificationsByIdQueryRequest request)
    {
      return await SendQuery<GetNotificationsByIdQueryRequest, GetNotificationsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Notifications Bildirimler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Notifications Bildirimler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Notifications Bildirimler bilgilerini içeren istek.</param> 
    /// <returns>Notifications Bildirimler bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Notifications Bildirimler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Notifications Bildirimler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Notifications Bildirimler Bilgilerini Görüntüle", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesNotificationsQueryResponse>>> GetAllDropboxesNotifications([FromQuery] GetAllDropboxesNotificationsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesNotificationsQueryRequest, GetAllDropboxesNotificationsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Notifications Bildirimler ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Notifications Bildirimler ekler.
    /// </remarks>
    /// <param name="request">Yeni Notifications Bildirimler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Notifications Bildirimler başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Notifications Bildirimler Eklemek", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<CreateNotificationsCommandResponse>>> CreateNotifications([FromBody] CreateNotificationsCommandRequest request)
    {
      return await SendCommand<CreateNotificationsCommandRequest, CreateNotificationsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Notifications Bildirimler kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Notifications Bildirimlernin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Notifications Bildirimler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Notifications Bildirimler başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Notifications Bildirimler bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Notifications Bildirimler Güncelemek", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<UpdateNotificationsCommandResponse>>> UpdateNotifications([FromBody] UpdateNotificationsCommandRequest request)
    {
      return await SendCommand<UpdateNotificationsCommandRequest, UpdateNotificationsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Notifications Bildirimler kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Notifications Bildirimler kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Notifications Bildirimler kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Notifications Bildirimler başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Notifications Bildirimler bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Notifications Bildirimler Silme", Menu = "Notifications-Bildirimler")]
    public async Task<ActionResult<TransactionResultPack<DeleteNotificationsCommandResponse>>> DeleteNotifications([FromRoute] DeleteNotificationsCommandRequest request)
    {
      return await SendCommand<DeleteNotificationsCommandRequest, DeleteNotificationsCommandResponse>(request);
    }
  }
}
