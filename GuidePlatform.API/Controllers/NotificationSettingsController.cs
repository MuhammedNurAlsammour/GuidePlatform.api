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
    /// Admin Ana Ekran NotificationSettings Bildirim Ayarları Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm NotificationSettings Bildirim Ayarlarılerin listesini getirir.
    /// 🔍 Ayar türü filtresi - Setting type filter
    /// setting_type 0:email, 1:push, 2:sms, 3:whatsapp, 4:telegram, etc.
    /// </remarks>
    /// <param name="request">Tüm NotificationSettings Bildirim Ayarlarıleri getirme parametrelerini içeren istek.</param>
    /// <returns>NotificationSettings Bildirim Ayarları listesini döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "NotificationSettings Bildirim Ayarları Listesi Getirir", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<GetAllNotificationSettingsQueryResponse>>> GetAllNotificationSettings([FromQuery] GetAllNotificationSettingsQueryRequest request)
    {
      return await SendQuery<GetAllNotificationSettingsQueryRequest, GetAllNotificationSettingsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre NotificationSettings Bildirim Ayarları bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir NotificationSettings Bildirim Ayarları kimliğine göre NotificationSettings Bildirim Ayarları bilgilerini getirir.
    /// </remarks>
    /// <param name="request">NotificationSettings Bildirim Ayarları kimliğini içeren istek.</param>
    /// <returns>NotificationSettings Bildirim Ayarları bilgilerini döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">NotificationSettings Bildirim Ayarları bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre NotificationSettings Bildirim Ayarları Bilgilerini Görüntüle", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<GetNotificationSettingsByIdQueryResponse>>> GetByIdNotificationSettings([FromQuery] GetNotificationSettingsByIdQueryRequest request)
    {
      return await SendQuery<GetNotificationSettingsByIdQueryRequest, GetNotificationSettingsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes NotificationSettings Bildirim Ayarları bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes NotificationSettings Bildirim Ayarları bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes NotificationSettings Bildirim Ayarları bilgilerini içeren istek.</param> 
    /// <returns>NotificationSettings Bildirim Ayarları bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes NotificationSettings Bildirim Ayarları bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">NotificationSettings Bildirim Ayarları bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes NotificationSettings Bildirim Ayarları Bilgilerini Görüntüle", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesNotificationSettingsQueryResponse>>> GetAllDropboxesNotificationSettings([FromQuery] GetAllDropboxesNotificationSettingsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesNotificationSettingsQueryRequest, GetAllDropboxesNotificationSettingsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir NotificationSettings Bildirim Ayarları ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir NotificationSettings Bildirim Ayarları ekler.
    /// </remarks>
    /// <param name="request">Yeni NotificationSettings Bildirim Ayarları bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">NotificationSettings Bildirim Ayarları başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "NotificationSettings Bildirim Ayarları Eklemek", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<CreateNotificationSettingsCommandResponse>>> CreateNotificationSettings([FromBody] CreateNotificationSettingsCommandRequest request)
    {
      return await SendCommand<CreateNotificationSettingsCommandRequest, CreateNotificationSettingsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir NotificationSettings Bildirim Ayarları kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip NotificationSettings Bildirim Ayarlarınin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek NotificationSettings Bildirim Ayarları bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek NotificationSettings Bildirim Ayarları bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "NotificationSettings Bildirim Ayarları Güncelemek", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<UpdateNotificationSettingsCommandResponse>>> UpdateNotificationSettings([FromBody] UpdateNotificationSettingsCommandRequest request)
    {
      return await SendCommand<UpdateNotificationSettingsCommandRequest, UpdateNotificationSettingsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip NotificationSettings Bildirim Ayarları kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip NotificationSettings Bildirim Ayarları kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek NotificationSettings Bildirim Ayarları kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">NotificationSettings Bildirim Ayarları başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek NotificationSettings Bildirim Ayarları bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "NotificationSettings Bildirim Ayarları Silme", Menu = "NotificationSettings-Bildirim Ayarları")]
    public async Task<ActionResult<TransactionResultPack<DeleteNotificationSettingsCommandResponse>>> DeleteNotificationSettings([FromRoute] DeleteNotificationSettingsCommandRequest request)
    {
      return await SendCommand<DeleteNotificationSettingsCommandRequest, DeleteNotificationSettingsCommandResponse>(request);
    }
  }
}
