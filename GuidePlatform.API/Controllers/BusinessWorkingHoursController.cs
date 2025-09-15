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
    /// Admin Ana Ekran BusinessWorkingHours İşletme Çalışma Saatleri Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessWorkingHours İşletme Çalışma Saatlerilerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessWorkingHours İşletme Çalışma Saatlerileri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri listesini döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri Listesi Getirir", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessWorkingHoursQueryResponse>>> GetAllBusinessWorkingHours([FromQuery] GetAllBusinessWorkingHoursQueryRequest request)
    {
      return await SendQuery<GetAllBusinessWorkingHoursQueryRequest, GetAllBusinessWorkingHoursQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessWorkingHours İşletme Çalışma Saatleri kimliğine göre BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessWorkingHours İşletme Çalışma Saatleri kimliğini içeren istek.</param>
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessWorkingHours İşletme Çalışma Saatleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessWorkingHours İşletme Çalışma Saatleri Bilgilerini Görüntüle", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessWorkingHoursByIdQueryResponse>>> GetByIdBusinessWorkingHours([FromQuery] GetBusinessWorkingHoursByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessWorkingHoursByIdQueryRequest, GetBusinessWorkingHoursByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini içeren istek.</param> 
    /// <returns>BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessWorkingHours İşletme Çalışma Saatleri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessWorkingHours İşletme Çalışma Saatleri Bilgilerini Görüntüle", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessWorkingHoursQueryResponse>>> GetAllDropboxesBusinessWorkingHours([FromQuery] GetAllDropboxesBusinessWorkingHoursQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessWorkingHoursQueryRequest, GetAllDropboxesBusinessWorkingHoursQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessWorkingHours İşletme Çalışma Saatleri ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessWorkingHours İşletme Çalışma Saatleri ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessWorkingHours İşletme Çalışma Saatleri başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri Eklemek", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessWorkingHoursCommandResponse>>> CreateBusinessWorkingHours([FromBody] CreateBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<CreateBusinessWorkingHoursCommandRequest, CreateBusinessWorkingHoursCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessWorkingHours İşletme Çalışma Saatleri kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatlerinin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessWorkingHours İşletme Çalışma Saatleri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessWorkingHours İşletme Çalışma Saatleri bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri Güncelemek", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessWorkingHoursCommandResponse>>> UpdateBusinessWorkingHours([FromBody] UpdateBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<UpdateBusinessWorkingHoursCommandRequest, UpdateBusinessWorkingHoursCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatleri kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessWorkingHours İşletme Çalışma Saatleri kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessWorkingHours İşletme Çalışma Saatleri kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessWorkingHours İşletme Çalışma Saatleri başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessWorkingHours İşletme Çalışma Saatleri bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessWorkingHours İşletme Çalışma Saatleri Silme", Menu = "BusinessWorkingHours-İşletme Çalışma Saatleri")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessWorkingHoursCommandResponse>>> DeleteBusinessWorkingHours([FromRoute] DeleteBusinessWorkingHoursCommandRequest request)
    {
      return await SendCommand<DeleteBusinessWorkingHoursCommandRequest, DeleteBusinessWorkingHoursCommandResponse>(request);
    }
  }
}
