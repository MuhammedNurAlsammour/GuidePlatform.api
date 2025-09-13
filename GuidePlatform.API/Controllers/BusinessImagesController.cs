using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessImages;
using GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImages;
using GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImagesWithImage;
using GuidePlatform.Application.Features.Commands.BusinessImages.DeleteBusinessImages;
using GuidePlatform.Application.Features.Commands.BusinessImages.UpdateBusinessImages;
using GuidePlatform.Application.Features.Queries.BusinessImages.GetAllBusinessImages;
using GuidePlatform.Application.Features.Queries.BusinessImages.GetBusinessImagesById;
using GuidePlatform.Application.Features.Queries.BusinessImages.GetAllDropboxesBusinessImages;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessImagesController : BaseController
  {
    public BusinessImagesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran BusinessImages İşletme Görselleri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessImages İşletme Görselleri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessImages İşletme Görselleri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessImages İşletme Görselleri tablosu listesini döndürür.</returns>
    /// <response code="200">BusinessImages İşletme Görselleri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessImages İşletme Görselleri tablosu Listesi Getirir", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessImagesQueryResponse>>> GetAllBusinessImages([FromQuery] GetAllBusinessImagesQueryRequest request)
    {
      return await SendQuery<GetAllBusinessImagesQueryRequest, GetAllBusinessImagesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessImages İşletme Görselleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessImages İşletme Görselleri tablosu kimliğine göre BusinessImages İşletme Görselleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessImages İşletme Görselleri tablosu kimliğini içeren istek.</param>
    /// <returns>BusinessImages İşletme Görselleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">BusinessImages İşletme Görselleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessImages İşletme Görselleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessImages İşletme Görselleri tablosu Bilgilerini Görüntüle", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessImagesByIdQueryResponse>>> GetByIdBusinessImages([FromQuery] GetBusinessImagesByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessImagesByIdQueryRequest, GetBusinessImagesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessImages İşletme Görselleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessImages İşletme Görselleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessImages İşletme Görselleri tablosu bilgilerini içeren istek.</param> 
    /// <returns>BusinessImages İşletme Görselleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessImages İşletme Görselleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessImages İşletme Görselleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessImages İşletme Görselleri tablosu Bilgilerini Görüntüle", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessImagesQueryResponse>>> GetAllDropboxesBusinessImages([FromQuery] GetAllDropboxesBusinessImagesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessImagesQueryRequest, GetAllDropboxesBusinessImagesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessImages İşletme Görselleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessImages İşletme Görselleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessImages İşletme Görselleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessImages İşletme Görselleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessImages İşletme Görselleri tablosu Eklemek", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessImagesCommandResponse>>> CreateBusinessImages([FromBody] CreateBusinessImagesCommandRequest request)
    {
      return await SendCommand<CreateBusinessImagesCommandRequest, CreateBusinessImagesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Gelen resim dosyasıyla yeni BusinessImages İşletme Görselleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, gelen resim dosyasıyla yeni BusinessImages İşletme Görselleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessImages İşletme Görselleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessImages İşletme Görselleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Resimli Yeni BusinessImages İşletme Görselleri tablosu Oluşturma", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessImagesWithImageCommandResponse>>> CreateBusinessImagesWithImage([FromForm] CreateBusinessImagesWithImageCommandRequest request)
    {
      return await SendCommand<CreateBusinessImagesWithImageCommandRequest, CreateBusinessImagesWithImageCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessImages İşletme Görselleri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessImages İşletme Görselleri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessImages İşletme Görselleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessImages İşletme Görselleri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessImages İşletme Görselleri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessImages İşletme Görselleri tablosu Güncelemek", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessImagesCommandResponse>>> UpdateBusinessImages([FromBody] UpdateBusinessImagesCommandRequest request)
    {
      return await SendCommand<UpdateBusinessImagesCommandRequest, UpdateBusinessImagesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessImages İşletme Görselleri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessImages İşletme Görselleri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessImages İşletme Görselleri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessImages İşletme Görselleri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessImages İşletme Görselleri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessImages İşletme Görselleri tablosu Silme", Menu = "BusinessImages İşletme Görselleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessImagesCommandResponse>>> DeleteBusinessImages([FromRoute] DeleteBusinessImagesCommandRequest request)
    {
      return await SendCommand<DeleteBusinessImagesCommandRequest, DeleteBusinessImagesCommandResponse>(request);
    }
  }
}
