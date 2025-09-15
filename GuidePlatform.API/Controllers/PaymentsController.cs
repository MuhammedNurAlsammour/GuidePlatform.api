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
    /// Admin Ana Ekran Payments Ödemeler Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Payments Ödemelerlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Payments Ödemelerleri getirme parametrelerini içeren istek.</param>
    /// <returns>Payments Ödemeler listesini döndürür.</returns>
    /// <response code="200">Payments Ödemeler listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Payments Ödemeler Listesi Getirir", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<GetAllPaymentsQueryResponse>>> GetAllPayments([FromQuery] GetAllPaymentsQueryRequest request)
    {
      return await SendQuery<GetAllPaymentsQueryRequest, GetAllPaymentsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Payments Ödemeler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Payments Ödemeler kimliğine göre Payments Ödemeler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Payments Ödemeler kimliğini içeren istek.</param>
    /// <returns>Payments Ödemeler bilgilerini döndürür.</returns>
    /// <response code="200">Payments Ödemeler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Payments Ödemeler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Payments Ödemeler Bilgilerini Görüntüle", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<GetPaymentsByIdQueryResponse>>> GetByIdPayments([FromQuery] GetPaymentsByIdQueryRequest request)
    {
      return await SendQuery<GetPaymentsByIdQueryRequest, GetPaymentsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Payments Ödemeler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Payments Ödemeler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Payments Ödemeler bilgilerini içeren istek.</param> 
    /// <returns>Payments Ödemeler bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Payments Ödemeler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Payments Ödemeler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Payments Ödemeler Bilgilerini Görüntüle", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesPaymentsQueryResponse>>> GetAllDropboxesPayments([FromQuery] GetAllDropboxesPaymentsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesPaymentsQueryRequest, GetAllDropboxesPaymentsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Payments Ödemeler ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Payments Ödemeler ekler.
    /// </remarks>
    /// <param name="request">Yeni Payments Ödemeler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Payments Ödemeler başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Payments Ödemeler Eklemek", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<CreatePaymentsCommandResponse>>> CreatePayments([FromBody] CreatePaymentsCommandRequest request)
    {
      return await SendCommand<CreatePaymentsCommandRequest, CreatePaymentsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Payments Ödemeler kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Payments Ödemelernin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Payments Ödemeler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Payments Ödemeler başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Payments Ödemeler bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Payments Ödemeler Güncelemek", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<UpdatePaymentsCommandResponse>>> UpdatePayments([FromBody] UpdatePaymentsCommandRequest request)
    {
      return await SendCommand<UpdatePaymentsCommandRequest, UpdatePaymentsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Payments Ödemeler kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Payments Ödemeler kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Payments Ödemeler kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Payments Ödemeler başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Payments Ödemeler bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Payments Ödemeler Silme", Menu = "Payments-Ödemeler")]
    public async Task<ActionResult<TransactionResultPack<DeletePaymentsCommandResponse>>> DeletePayments([FromRoute] DeletePaymentsCommandRequest request)
    {
      return await SendCommand<DeletePaymentsCommandRequest, DeletePaymentsCommandResponse>(request);
    }
  }
}
