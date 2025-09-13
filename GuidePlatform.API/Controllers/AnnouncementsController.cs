using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Announcements;
using GuidePlatform.Application.Features.Commands.Announcements.CreateAnnouncements;
using GuidePlatform.Application.Features.Commands.Announcements.DeleteAnnouncements;
using GuidePlatform.Application.Features.Commands.Announcements.UpdateAnnouncements;
using GuidePlatform.Application.Features.Queries.Announcements.GetAllAnnouncements;
using GuidePlatform.Application.Features.Queries.Announcements.GetAnnouncementsById;
using GuidePlatform.Application.Features.Queries.Announcements.GetAllDropboxesAnnouncements;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class AnnouncementsController : BaseController
  {
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Announcements Duyurular tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Announcements Duyurular tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Announcements Duyurular tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Announcements Duyurular tablosu listesini döndürür.</returns>
    /// <response code="200">Announcements Duyurular tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Announcements Duyurular tablosu Listesi Getirir", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllAnnouncementsQueryResponse>>> GetAllAnnouncements([FromQuery] GetAllAnnouncementsQueryRequest request)
    {
      return await SendQuery<GetAllAnnouncementsQueryRequest, GetAllAnnouncementsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Announcements Duyurular tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Announcements Duyurular tablosu kimliğine göre Announcements Duyurular tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Announcements Duyurular tablosu kimliğini içeren istek.</param>
    /// <returns>Announcements Duyurular tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Announcements Duyurular tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Announcements Duyurular tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Announcements Duyurular tablosu Bilgilerini Görüntüle", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAnnouncementsByIdQueryResponse>>> GetByIdAnnouncements([FromQuery] GetAnnouncementsByIdQueryRequest request)
    {
      return await SendQuery<GetAnnouncementsByIdQueryRequest, GetAnnouncementsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Announcements Duyurular tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Announcements Duyurular tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Announcements Duyurular tablosu bilgilerini içeren istek.</param> 
    /// <returns>Announcements Duyurular tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Announcements Duyurular tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Announcements Duyurular tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Announcements Duyurular tablosu Bilgilerini Görüntüle", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesAnnouncementsQueryResponse>>> GetAllDropboxesAnnouncements([FromQuery] GetAllDropboxesAnnouncementsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesAnnouncementsQueryRequest, GetAllDropboxesAnnouncementsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Announcements Duyurular tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Announcements Duyurular tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Announcements Duyurular tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Announcements Duyurular tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Announcements Duyurular tablosu Eklemek", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateAnnouncementsCommandResponse>>> CreateAnnouncements([FromBody] CreateAnnouncementsCommandRequest request)
    {
      return await SendCommand<CreateAnnouncementsCommandRequest, CreateAnnouncementsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Announcements Duyurular tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Announcements Duyurular tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Announcements Duyurular tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Announcements Duyurular tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Announcements Duyurular tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Announcements Duyurular tablosu Güncelemek", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateAnnouncementsCommandResponse>>> UpdateAnnouncements([FromBody] UpdateAnnouncementsCommandRequest request)
    {
      return await SendCommand<UpdateAnnouncementsCommandRequest, UpdateAnnouncementsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Announcements Duyurular tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Announcements Duyurular tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Announcements Duyurular tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Announcements Duyurular tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Announcements Duyurular tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Announcements Duyurular tablosu Silme", Menu = "Announcements Duyurular tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteAnnouncementsCommandResponse>>> DeleteAnnouncements([FromRoute] DeleteAnnouncementsCommandRequest request)
    {
      return await SendCommand<DeleteAnnouncementsCommandRequest, DeleteAnnouncementsCommandResponse>(request);
    }
  }
}
