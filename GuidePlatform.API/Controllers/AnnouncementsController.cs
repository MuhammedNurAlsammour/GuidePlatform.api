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
    /// Admin Ana Ekran Announcements Duyurular Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Announcements Duyurularlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Announcements Duyurularleri getirme parametrelerini içeren istek.</param>
    /// <returns>Announcements Duyurular listesini döndürür.</returns>
    /// <response code="200">Announcements Duyurular listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Announcements Duyurular Listesi Getirir", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<GetAllAnnouncementsQueryResponse>>> GetAllAnnouncements([FromQuery] GetAllAnnouncementsQueryRequest request)
    {
      return await SendQuery<GetAllAnnouncementsQueryRequest, GetAllAnnouncementsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Announcements Duyurular bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Announcements Duyurular kimliğine göre Announcements Duyurular bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Announcements Duyurular kimliğini içeren istek.</param>
    /// <returns>Announcements Duyurular bilgilerini döndürür.</returns>
    /// <response code="200">Announcements Duyurular bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Announcements Duyurular bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Announcements Duyurular Bilgilerini Görüntüle", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<GetAnnouncementsByIdQueryResponse>>> GetByIdAnnouncements([FromQuery] GetAnnouncementsByIdQueryRequest request)
    {
      return await SendQuery<GetAnnouncementsByIdQueryRequest, GetAnnouncementsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Announcements Duyurular bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Announcements Duyurular bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Announcements Duyurular bilgilerini içeren istek.</param> 
    /// <returns>Announcements Duyurular bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Announcements Duyurular bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Announcements Duyurular bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Announcements Duyurular Bilgilerini Görüntüle", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesAnnouncementsQueryResponse>>> GetAllDropboxesAnnouncements([FromQuery] GetAllDropboxesAnnouncementsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesAnnouncementsQueryRequest, GetAllDropboxesAnnouncementsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Announcements Duyurular ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Announcements Duyurular ekler.
    /// </remarks>
    /// <param name="request">Yeni Announcements Duyurular bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Announcements Duyurular başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Announcements Duyurular Eklemek", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<CreateAnnouncementsCommandResponse>>> CreateAnnouncements([FromBody] CreateAnnouncementsCommandRequest request)
    {
      return await SendCommand<CreateAnnouncementsCommandRequest, CreateAnnouncementsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Announcements Duyurular kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Announcements Duyurularnin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Announcements Duyurular bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Announcements Duyurular başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Announcements Duyurular bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Announcements Duyurular Güncelemek", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<UpdateAnnouncementsCommandResponse>>> UpdateAnnouncements([FromBody] UpdateAnnouncementsCommandRequest request)
    {
      return await SendCommand<UpdateAnnouncementsCommandRequest, UpdateAnnouncementsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Announcements Duyurular kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Announcements Duyurular kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Announcements Duyurular kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Announcements Duyurular başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Announcements Duyurular bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Announcements Duyurular Silme", Menu = "Announcements-Duyurular")]
    public async Task<ActionResult<TransactionResultPack<DeleteAnnouncementsCommandResponse>>> DeleteAnnouncements([FromRoute] DeleteAnnouncementsCommandRequest request)
    {
      return await SendCommand<DeleteAnnouncementsCommandRequest, DeleteAnnouncementsCommandResponse>(request);
    }
  }
}
