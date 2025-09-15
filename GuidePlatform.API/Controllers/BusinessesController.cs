using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Businesses;
using GuidePlatform.Application.Features.Commands.Businesses.CreateBusinesses;
using GuidePlatform.Application.Features.Commands.Businesses.DeleteBusinesses;
using GuidePlatform.Application.Features.Commands.Businesses.UpdateBusinesses;
using GuidePlatform.Application.Features.Commands.Businesses.IncrementViewCount;
using GuidePlatform.Application.Features.Queries.Businesses.GetAllBusinesses;
using GuidePlatform.Application.Features.Queries.Businesses.GetBusinessesById;
using GuidePlatform.Application.Features.Queries.Businesses.GetAllDropboxesBusinesses;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessesController : BaseController
  {
    public BusinessesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Businesses İşletmeler ve Organizasyonlar Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Businesses İşletmeler ve Organizasyonlarlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Businesses İşletmeler ve Organizasyonlarleri getirme parametrelerini içeren istek.</param>
    /// <returns>Businesses İşletmeler ve Organizasyonlar listesini döndürür.</returns>
    /// <response code="200">Businesses İşletmeler ve Organizasyonlar listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Businesses İşletmeler ve Organizasyonlar Listesi Getirir", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessesQueryResponse>>> GetAllBusinesses([FromQuery] GetAllBusinessesQueryRequest request)
    {
      return await SendQuery<GetAllBusinessesQueryRequest, GetAllBusinessesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Businesses İşletmeler ve Organizasyonlar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Businesses İşletmeler ve Organizasyonlar kimliğine göre Businesses İşletmeler ve Organizasyonlar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Businesses İşletmeler ve Organizasyonlar kimliğini içeren istek.</param>
    /// <returns>Businesses İşletmeler ve Organizasyonlar bilgilerini döndürür.</returns>
    /// <response code="200">Businesses İşletmeler ve Organizasyonlar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Businesses İşletmeler ve Organizasyonlar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Businesses İşletmeler ve Organizasyonlar Bilgilerini Görüntüle", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessesByIdQueryResponse>>> GetByIdBusinesses([FromQuery] GetBusinessesByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessesByIdQueryRequest, GetBusinessesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Businesses İşletmeler ve Organizasyonlar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Businesses İşletmeler ve Organizasyonlar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Businesses İşletmeler ve Organizasyonlar bilgilerini içeren istek.</param> 
    /// <returns>Businesses İşletmeler ve Organizasyonlar bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Businesses İşletmeler ve Organizasyonlar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Businesses İşletmeler ve Organizasyonlar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Businesses İşletmeler ve Organizasyonlar Bilgilerini Görüntüle", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessesQueryResponse>>> GetAllDropboxesBusinesses([FromQuery] GetAllDropboxesBusinessesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessesQueryRequest, GetAllDropboxesBusinessesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Businesses İşletmeler ve Organizasyonlar ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Businesses İşletmeler ve Organizasyonlar ekler.
    /// </remarks>
    /// <param name="request">Yeni Businesses İşletmeler ve Organizasyonlar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Businesses İşletmeler ve Organizasyonlar başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Businesses İşletmeler ve Organizasyonlar Eklemek", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessesCommandResponse>>> CreateBusinesses([FromBody] CreateBusinessesCommandRequest request)
    {
      return await SendCommand<CreateBusinessesCommandRequest, CreateBusinessesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Businesses İşletmeler ve Organizasyonlar kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Businesses İşletmeler ve Organizasyonlarnin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Businesses İşletmeler ve Organizasyonlar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Businesses İşletmeler ve Organizasyonlar başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Businesses İşletmeler ve Organizasyonlar bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Businesses İşletmeler ve Organizasyonlar Güncelemek", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessesCommandResponse>>> UpdateBusinesses([FromBody] UpdateBusinessesCommandRequest request)
    {
      return await SendCommand<UpdateBusinessesCommandRequest, UpdateBusinessesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Businesses İşletmeler ve Organizasyonlar kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Businesses İşletmeler ve Organizasyonlar kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Businesses İşletmeler ve Organizasyonlar kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Businesses İşletmeler ve Organizasyonlar başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Businesses İşletmeler ve Organizasyonlar bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Businesses İşletmeler ve Organizasyonlar Silme", Menu = "Businesses-İşletmeler ve Organizasyonlar")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessesCommandResponse>>> DeleteBusinesses([FromRoute] DeleteBusinessesCommandRequest request)
    {
      return await SendCommand<DeleteBusinessesCommandRequest, DeleteBusinessesCommandResponse>(request);
    }

    /// <summary>
    /// İşletme görüntüleme sayısını bir artırır (ViewCount).
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen işletmenin görüntüleme sayısını bir artırır. 
    /// Kullanıcı işletme detay sayfasını ziyaret ettiğinde çağrılmalıdır.
    /// Yetkilendirme gerektirmez - herkese açık endpoint.
    /// </remarks>
    /// <param name="businessId">Görüntüleme sayısı artırılacak işletme ID'si.</param>
    /// <returns>İşlem sonucunu ve yeni görüntüleme sayısını döndürür.</returns>
    /// <response code="200">Görüntüleme sayısı başarıyla artırıldı.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="404">İşletme bulunamazsa.</response>
    [HttpPatch("[action]/{businessId}")]
    [AllowAnonymous] // Herkese açık - yetkilendirme gerektirmez
    public async Task<ActionResult<TransactionResultPack<IncrementViewCountCommandResponse>>> IncrementViewCount([FromRoute] Guid businessId)
    {
      // İsteğe bağlı: Client bilgilerini al (analytics için)
      var request = new IncrementViewCountCommandRequest
      {
        BusinessId = businessId,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault(),
        RefererUrl = HttpContext.Request.Headers["Referer"].FirstOrDefault()
      };

      return await SendCommand<IncrementViewCountCommandRequest, IncrementViewCountCommandResponse>(request);
    }
  }
}
