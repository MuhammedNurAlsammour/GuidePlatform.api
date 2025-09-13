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
    /// Admin Ana Ekran Notifications Bildirimler tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Notifications Bildirimler tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Notifications Bildirimler tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Notifications Bildirimler tablosu listesini döndürür.</returns>
    /// <response code="200">Notifications Bildirimler tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Notifications Bildirimler tablosu Listesi Getirir", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllNotificationsQueryResponse>>> GetAllNotifications([FromQuery] GetAllNotificationsQueryRequest request)
    {
      return await SendQuery<GetAllNotificationsQueryRequest, GetAllNotificationsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Notifications Bildirimler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Notifications Bildirimler tablosu kimliğine göre Notifications Bildirimler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Notifications Bildirimler tablosu kimliğini içeren istek.</param>
    /// <returns>Notifications Bildirimler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Notifications Bildirimler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Notifications Bildirimler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Notifications Bildirimler tablosu Bilgilerini Görüntüle", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetNotificationsByIdQueryResponse>>> GetByIdNotifications([FromQuery] GetNotificationsByIdQueryRequest request)
    {
      return await SendQuery<GetNotificationsByIdQueryRequest, GetNotificationsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Notifications Bildirimler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Notifications Bildirimler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Notifications Bildirimler tablosu bilgilerini içeren istek.</param> 
    /// <returns>Notifications Bildirimler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Notifications Bildirimler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Notifications Bildirimler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Notifications Bildirimler tablosu Bilgilerini Görüntüle", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesNotificationsQueryResponse>>> GetAllDropboxesNotifications([FromQuery] GetAllDropboxesNotificationsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesNotificationsQueryRequest, GetAllDropboxesNotificationsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Notifications Bildirimler tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Notifications Bildirimler tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Notifications Bildirimler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Notifications Bildirimler tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Notifications Bildirimler tablosu Eklemek", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateNotificationsCommandResponse>>> CreateNotifications([FromBody] CreateNotificationsCommandRequest request)
    {
      return await SendCommand<CreateNotificationsCommandRequest, CreateNotificationsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Notifications Bildirimler tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Notifications Bildirimler tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Notifications Bildirimler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Notifications Bildirimler tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Notifications Bildirimler tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Notifications Bildirimler tablosu Güncelemek", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateNotificationsCommandResponse>>> UpdateNotifications([FromBody] UpdateNotificationsCommandRequest request)
    {
      return await SendCommand<UpdateNotificationsCommandRequest, UpdateNotificationsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Notifications Bildirimler tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Notifications Bildirimler tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Notifications Bildirimler tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Notifications Bildirimler tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Notifications Bildirimler tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Notifications Bildirimler tablosu Silme", Menu = "Notifications Bildirimler tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteNotificationsCommandResponse>>> DeleteNotifications([FromRoute] DeleteNotificationsCommandRequest request)
    {
      return await SendCommand<DeleteNotificationsCommandRequest, DeleteNotificationsCommandResponse>(request);
    }
  }
}
