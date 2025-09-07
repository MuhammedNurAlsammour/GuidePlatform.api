using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Features.Commands.Banners.CreateBanners;
using GuidePlatform.Application.Features.Commands.Banners.DeleteBanners;
using GuidePlatform.Application.Features.Commands.Banners.UpdateBanners;
using GuidePlatform.Application.Features.Queries.Banners.GetAllBanners;
using GuidePlatform.Application.Features.Queries.Banners.GetBannersById;
using GuidePlatform.Application.Features.Queries.Banners.GetAllDropboxesBanners;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BannersController : BaseController
  {
    public BannersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Banners Bannerlar tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Banners Bannerlar tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Banners Bannerlar tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Banners Bannerlar tablosu listesini döndürür.</returns>
    /// <response code="200">Banners Bannerlar tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Banners Bannerlar tablosu Listesi Getirir", Menu = "Banners Bannerlar tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBannersQueryResponse>>> GetAllBanners([FromQuery] GetAllBannersQueryRequest request)
    {
      return await SendQuery<GetAllBannersQueryRequest, GetAllBannersQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Banners Bannerlar tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Banners Bannerlar tablosu kimliğine göre Banners Bannerlar tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Banners Bannerlar tablosu kimliğini içeren istek.</param>
    /// <returns>Banners Bannerlar tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Banners Bannerlar tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Banners Bannerlar tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Banners Bannerlar tablosu Bilgilerini Görüntüle", Menu = "Banners Bannerlar tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBannersByIdQueryResponse>>> GetByIdBanners([FromQuery] GetBannersByIdQueryRequest request)
    {
      return await SendQuery<GetBannersByIdQueryRequest, GetBannersByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Banners Bannerlar tablosu tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Banners Bannerlar tablosu tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Banners Bannerlar tablosu tablosu bilgilerini içeren istek.</param> 
    /// <returns>Banners Bannerlar tablosu tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Banners Bannerlar tablosu tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Banners Bannerlar tablosu tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Banners Bannerlar tablosu tablosu Bilgilerini Görüntüle", Menu = "Banners Bannerlar tablosu tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBannersQueryResponse>>> GetAllDropboxesBanners([FromQuery] GetAllDropboxesBannersQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBannersQueryRequest, GetAllDropboxesBannersQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir Banners Bannerlar tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Banners Bannerlar tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Banners Bannerlar tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Banners Bannerlar tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Banners Bannerlar tablosu Eklemek", Menu = "Banners Bannerlar tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBannersCommandResponse>>> CreateBanners([FromBody] CreateBannersCommandRequest request)
    {
      return await SendCommand<CreateBannersCommandRequest, CreateBannersCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Banners Bannerlar tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Banners Bannerlar tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Banners Bannerlar tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Banners Bannerlar tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Banners Bannerlar tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Banners Bannerlar tablosu Güncelemek", Menu = "Banners Bannerlar tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBannersCommandResponse>>> UpdateBanners([FromBody] UpdateBannersCommandRequest request)
    {
      return await SendCommand<UpdateBannersCommandRequest, UpdateBannersCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Banners Bannerlar tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Banners Bannerlar tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Banners Bannerlar tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Banners Bannerlar tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Banners Bannerlar tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Banners Bannerlar tablosu Silme", Menu = "Banners Bannerlar tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBannersCommandResponse>>> DeleteBanners([FromRoute] DeleteBannersCommandRequest request)
    {
      return await SendCommand<DeleteBannersCommandRequest, DeleteBannersCommandResponse>(request);
    }
  }
}
