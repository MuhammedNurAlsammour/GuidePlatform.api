using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.NotificationSettings;
using GuidePlatform.Application.Features.Commands.NotificationSettings.CreateNotificationSettings;
using GuidePlatform.Application.Features.Commands.NotificationSettings.DeleteNotificationSettings;
using GuidePlatform.Application.Features.Commands.NotificationSettings.UpdateNotificationSettings;
using GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllNotificationSettings;
using GuidePlatform.Application.Features.Queries.NotificationSettings.GetNotificationSettingsById;
using GuidePlatform.Application.Features.Queries.NotificationSettings.GetAllDropboxesNotificationSettings;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class NotificationSettingsController : BaseController
  {
    public NotificationSettingsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran NotificationSettings Bildirim Ayarları tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm NotificationSettings Bildirim Ayarları tablosulerin listesini getirir.
    /// 🔍 Ayar türü filtresi - Setting type filter
    /// setting_type 0:email, 1:push, 2:sms, 3:whatsapp, 4:telegram, etc.
    /// </remarks>
    /// <param name="request">Tüm NotificationSettings Bildirim Ayarları tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>NotificationSettings Bildirim Ayarları tablosu listesini döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "NotificationSettings Bildirim Ayarları tablosu Listesi Getirir", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllNotificationSettingsQueryResponse>>> GetAllNotificationSettings([FromQuery] GetAllNotificationSettingsQueryRequest request)
    {
      return await SendQuery<GetAllNotificationSettingsQueryRequest, GetAllNotificationSettingsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre NotificationSettings Bildirim Ayarları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir NotificationSettings Bildirim Ayarları tablosu kimliğine göre NotificationSettings Bildirim Ayarları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">NotificationSettings Bildirim Ayarları tablosu kimliğini içeren istek.</param>
    /// <returns>NotificationSettings Bildirim Ayarları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">NotificationSettings Bildirim Ayarları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre NotificationSettings Bildirim Ayarları tablosu Bilgilerini Görüntüle", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetNotificationSettingsByIdQueryResponse>>> GetByIdNotificationSettings([FromQuery] GetNotificationSettingsByIdQueryRequest request)
    {
      return await SendQuery<GetNotificationSettingsByIdQueryRequest, GetNotificationSettingsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes NotificationSettings Bildirim Ayarları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes NotificationSettings Bildirim Ayarları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes NotificationSettings Bildirim Ayarları tablosu bilgilerini içeren istek.</param> 
    /// <returns>NotificationSettings Bildirim Ayarları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes NotificationSettings Bildirim Ayarları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">NotificationSettings Bildirim Ayarları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes NotificationSettings Bildirim Ayarları tablosu Bilgilerini Görüntüle", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesNotificationSettingsQueryResponse>>> GetAllDropboxesNotificationSettings([FromQuery] GetAllDropboxesNotificationSettingsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesNotificationSettingsQueryRequest, GetAllDropboxesNotificationSettingsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir NotificationSettings Bildirim Ayarları tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir NotificationSettings Bildirim Ayarları tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni NotificationSettings Bildirim Ayarları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">NotificationSettings Bildirim Ayarları tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "NotificationSettings Bildirim Ayarları tablosu Eklemek", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateNotificationSettingsCommandResponse>>> CreateNotificationSettings([FromBody] CreateNotificationSettingsCommandRequest request)
    {
      return await SendCommand<CreateNotificationSettingsCommandRequest, CreateNotificationSettingsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir NotificationSettings Bildirim Ayarları tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip NotificationSettings Bildirim Ayarları tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek NotificationSettings Bildirim Ayarları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek NotificationSettings Bildirim Ayarları tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "NotificationSettings Bildirim Ayarları tablosu Güncelemek", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateNotificationSettingsCommandResponse>>> UpdateNotificationSettings([FromBody] UpdateNotificationSettingsCommandRequest request)
    {
      return await SendCommand<UpdateNotificationSettingsCommandRequest, UpdateNotificationSettingsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip NotificationSettings Bildirim Ayarları tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip NotificationSettings Bildirim Ayarları tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek NotificationSettings Bildirim Ayarları tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek NotificationSettings Bildirim Ayarları tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "NotificationSettings Bildirim Ayarları tablosu Silme", Menu = "NotificationSettings Bildirim Ayarları tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteNotificationSettingsCommandResponse>>> DeleteNotificationSettings([FromRoute] DeleteNotificationSettingsCommandRequest request)
    {
      return await SendCommand<DeleteNotificationSettingsCommandRequest, DeleteNotificationSettingsCommandResponse>(request);
    }
  }
}
