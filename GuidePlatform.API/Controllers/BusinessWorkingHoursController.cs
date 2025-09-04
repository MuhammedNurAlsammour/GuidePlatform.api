using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessWorkingHours;
using GuidePlatform.Application.Features.Commands.BusinessWorkingHours.CreateBusinessWorkingHours;
using GuidePlatform.Application.Features.Commands.BusinessWorkingHours.DeleteBusinessWorkingHours;
using GuidePlatform.Application.Features.Commands.BusinessWorkingHours.UpdateBusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllBusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetBusinessWorkingHoursById;
using GuidePlatform.Application.Features.Queries.BusinessWorkingHours.GetAllDropboxesBusinessWorkingHours;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessWorkingHoursController : BaseController
  {
    public BusinessWorkingHoursController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran BusinessWorkingHours İşletme Çalışma Saatleri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessWorkingHours İşletme Çalışma Saatleri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessWorkingHours İşletme Çalışma Saatleri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri tablosu listesini döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu Listesi Getirir", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>>> GetAllBusinessWorkingHours([FromQuery] GetAllBusinessWorkingHoursQueryRequest request)
    {
      return await SendQuery<GetAllBusinessWorkingHoursQueryRequest, GetAllBusinessWorkingHoursQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessWorkingHours İşletme Çalışma Saatleri tablosu kimliğine göre BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessWorkingHours İşletme Çalışma Saatleri tablosu kimliğini içeren istek.</param>
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessWorkingHours İşletme Çalışma Saatleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessWorkingHours İşletme Çalışma Saatleri tablosu Bilgilerini Görüntüle", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessWorkingHoursByIdQueryResponse>>> GetByIdBusinessWorkingHours([FromQuery] GetBusinessWorkingHoursByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessWorkingHoursByIdQueryRequest, GetBusinessWorkingHoursByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bilgilerini içeren istek.</param> 
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu Bilgilerini Görüntüle", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessWorkingHoursQueryResponse>>> GetAllDropboxesBusinessWorkingHours([FromQuery] GetAllDropboxesBusinessWorkingHoursQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessWorkingHoursQueryRequest, GetAllDropboxesBusinessWorkingHoursQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir BusinessWorkingHours İşletme Çalışma Saatleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessWorkingHours İşletme Çalışma Saatleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessWorkingHours İşletme Çalışma Saatleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu Eklemek", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessWorkingHoursCommandResponse>>> CreateBusinessWorkingHours([FromBody] CreateBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<CreateBusinessWorkingHoursCommandRequest, CreateBusinessWorkingHoursCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessWorkingHours İşletme Çalışma Saatleri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatleri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessWorkingHours İşletme Çalışma Saatleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessWorkingHours İşletme Çalışma Saatleri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu Güncelemek", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessWorkingHoursCommandResponse>>> UpdateBusinessWorkingHours([FromBody] UpdateBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<UpdateBusinessWorkingHoursCommandRequest, UpdateBusinessWorkingHoursCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatleri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatleri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessWorkingHours İşletme Çalışma Saatleri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessWorkingHours İşletme Çalışma Saatleri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu Silme", Menu = "BusinessWorkingHours İşletme Çalışma Saatleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessWorkingHoursCommandResponse>>> DeleteBusinessWorkingHours([FromRoute] DeleteBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<DeleteBusinessWorkingHoursCommandRequest, DeleteBusinessWorkingHoursCommandResponse>(request);
    }
  }
}
