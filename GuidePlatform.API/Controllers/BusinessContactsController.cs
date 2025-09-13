using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessContacts;
using GuidePlatform.Application.Features.Commands.BusinessContacts.CreateBusinessContacts;
using GuidePlatform.Application.Features.Commands.BusinessContacts.DeleteBusinessContacts;
using GuidePlatform.Application.Features.Commands.BusinessContacts.UpdateBusinessContacts;
using GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllBusinessContacts;
using GuidePlatform.Application.Features.Queries.BusinessContacts.GetBusinessContactsById;
using GuidePlatform.Application.Features.Queries.BusinessContacts.GetAllDropboxesBusinessContacts;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessContactsController : BaseController
  {
    public BusinessContactsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran BusinessContacts İşletme İletişim Bilgileri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessContacts İşletme İletişim Bilgileri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessContacts İşletme İletişim Bilgileri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessContacts İşletme İletişim Bilgileri tablosu listesini döndürür.</returns>
    /// <response code="200">BusinessContacts İşletme İletişim Bilgileri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessContacts İşletme İletişim Bilgileri tablosu Listesi Getirir", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessContactsQueryResponse>>> GetAllBusinessContacts([FromQuery] GetAllBusinessContactsQueryRequest request)
    {
      return await SendQuery<GetAllBusinessContactsQueryRequest, GetAllBusinessContactsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessContacts İşletme İletişim Bilgileri tablosu kimliğine göre BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessContacts İşletme İletişim Bilgileri tablosu kimliğini içeren istek.</param>
    /// <returns>BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessContacts İşletme İletişim Bilgileri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessContacts İşletme İletişim Bilgileri tablosu Bilgilerini Görüntüle", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessContactsByIdQueryResponse>>> GetByIdBusinessContacts([FromQuery] GetBusinessContactsByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessContactsByIdQueryRequest, GetBusinessContactsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini içeren istek.</param> 
    /// <returns>BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessContacts İşletme İletişim Bilgileri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessContacts İşletme İletişim Bilgileri tablosu Bilgilerini Görüntüle", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessContactsQueryResponse>>> GetAllDropboxesBusinessContacts([FromQuery] GetAllDropboxesBusinessContactsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessContactsQueryRequest, GetAllDropboxesBusinessContactsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessContacts İşletme İletişim Bilgileri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessContacts İşletme İletişim Bilgileri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessContacts İşletme İletişim Bilgileri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessContacts İşletme İletişim Bilgileri tablosu Eklemek", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessContactsCommandResponse>>> CreateBusinessContacts([FromBody] CreateBusinessContactsCommandRequest request)
    {
      return await SendCommand<CreateBusinessContactsCommandRequest, CreateBusinessContactsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessContacts İşletme İletişim Bilgileri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessContacts İşletme İletişim Bilgileri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessContacts İşletme İletişim Bilgileri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessContacts İşletme İletişim Bilgileri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessContacts İşletme İletişim Bilgileri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessContacts İşletme İletişim Bilgileri tablosu Güncelemek", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessContactsCommandResponse>>> UpdateBusinessContacts([FromBody] UpdateBusinessContactsCommandRequest request)
    {
      return await SendCommand<UpdateBusinessContactsCommandRequest, UpdateBusinessContactsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessContacts İşletme İletişim Bilgileri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessContacts İşletme İletişim Bilgileri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessContacts İşletme İletişim Bilgileri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessContacts İşletme İletişim Bilgileri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessContacts İşletme İletişim Bilgileri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessContacts İşletme İletişim Bilgileri tablosu Silme", Menu = "BusinessContacts İşletme İletişim Bilgileri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessContactsCommandResponse>>> DeleteBusinessContacts([FromRoute] DeleteBusinessContactsCommandRequest request)
    {
      return await SendCommand<DeleteBusinessContactsCommandRequest, DeleteBusinessContactsCommandResponse>(request);
    }
  }
}
