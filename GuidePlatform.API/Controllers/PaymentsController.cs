using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Payments;
using GuidePlatform.Application.Features.Commands.Payments.CreatePayments;
using GuidePlatform.Application.Features.Commands.Payments.DeletePayments;
using GuidePlatform.Application.Features.Commands.Payments.UpdatePayments;
using GuidePlatform.Application.Features.Queries.Payments.GetAllPayments;
using GuidePlatform.Application.Features.Queries.Payments.GetPaymentsById;
using GuidePlatform.Application.Features.Queries.Payments.GetAllDropboxesPayments;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class PaymentsController : BaseController
  {
    public PaymentsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Payments Ödemeler tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Payments Ödemeler tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Payments Ödemeler tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Payments Ödemeler tablosu listesini döndürür.</returns>
    /// <response code="200">Payments Ödemeler tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Payments Ödemeler tablosu Listesi Getirir", Menu = "Payments Ödemeler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllPaymentsQueryResponse>>> GetAllPayments([FromQuery] GetAllPaymentsQueryRequest request)
    {
      return await SendQuery<GetAllPaymentsQueryRequest, GetAllPaymentsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Payments Ödemeler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Payments Ödemeler tablosu kimliğine göre Payments Ödemeler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Payments Ödemeler tablosu kimliğini içeren istek.</param>
    /// <returns>Payments Ödemeler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Payments Ödemeler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Payments Ödemeler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Payments Ödemeler tablosu Bilgilerini Görüntüle", Menu = "Payments Ödemeler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetPaymentsByIdQueryResponse>>> GetByIdPayments([FromQuery] GetPaymentsByIdQueryRequest request)
    {
      return await SendQuery<GetPaymentsByIdQueryRequest, GetPaymentsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Payments Ödemeler tablosu tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Payments Ödemeler tablosu tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Payments Ödemeler tablosu tablosu bilgilerini içeren istek.</param> 
    /// <returns>Payments Ödemeler tablosu tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Payments Ödemeler tablosu tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Payments Ödemeler tablosu tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Payments Ödemeler tablosu tablosu Bilgilerini Görüntüle", Menu = "Payments Ödemeler tablosu tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesPaymentsQueryResponse>>> GetAllDropboxesPayments([FromQuery] GetAllDropboxesPaymentsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesPaymentsQueryRequest, GetAllDropboxesPaymentsQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir Payments Ödemeler tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Payments Ödemeler tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Payments Ödemeler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Payments Ödemeler tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Payments Ödemeler tablosu Eklemek", Menu = "Payments Ödemeler tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreatePaymentsCommandResponse>>> CreatePayments([FromBody] CreatePaymentsCommandRequest request)
    {
      return await SendCommand<CreatePaymentsCommandRequest, CreatePaymentsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Payments Ödemeler tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Payments Ödemeler tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Payments Ödemeler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Payments Ödemeler tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Payments Ödemeler tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Payments Ödemeler tablosu Güncelemek", Menu = "Payments Ödemeler tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdatePaymentsCommandResponse>>> UpdatePayments([FromBody] UpdatePaymentsCommandRequest request)
    {
      return await SendCommand<UpdatePaymentsCommandRequest, UpdatePaymentsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Payments Ödemeler tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Payments Ödemeler tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Payments Ödemeler tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Payments Ödemeler tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Payments Ödemeler tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Payments Ödemeler tablosu Silme", Menu = "Payments Ödemeler tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeletePaymentsCommandResponse>>> DeletePayments([FromRoute] DeletePaymentsCommandRequest request)
    {
      return await SendCommand<DeletePaymentsCommandRequest, DeletePaymentsCommandResponse>(request);
    }
  }
}
