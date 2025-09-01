using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.categories;
using GuidePlatform.Application.Features.Commands.categories.DeleteCategories;
using GuidePlatform.Application.Features.Commands.categories.UpdateCategories;
using GuidePlatform.Application.Features.Queries.categories.GetAllCategories;
using GuidePlatform.Application.Features.Queries.categories.GetCategoriesById;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;
using GuidePlatform.Application.Features.Queries.categories.GetAllCategories;
using GuidePlatform.Application.Features.Commands.categories.CreateCategories;
using GuidePlatform.Application.Features.Queries.categories.GetAllDropboxesCategories;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class categoriesController : BaseController
  {
    public categoriesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Kategoriler tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Kategoriler tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Kategoriler tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Kategoriler tablosu listesini döndürür.</returns>
    /// <response code="200">Kategoriler tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Kategoriler tablosu Listesi Getirir", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllCategoriesQueryResponse>>> GetAllcategories([FromQuery] GetAllCategoriesQueryRequest request)
    {
      return await SendQuery<GetAllCategoriesQueryRequest, GetAllCategoriesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Kategoriler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Kategoriler tablosu kimliğine göre Kategoriler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Kategoriler tablosu kimliğini içeren istek.</param>
    /// <returns>Kategoriler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Kategoriler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Kategoriler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Kategoriler tablosu Bilgilerini Görüntüle", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetCategoriesByIdQueryResponse>>> GetByIdcategories([FromQuery] GetCategoriesByIdQueryRequest request)
    {
      return await SendQuery<GetCategoriesByIdQueryRequest, GetCategoriesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Kategoriler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Kategoriler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Kategoriler tablosu bilgilerini içeren istek.</param>
    /// <returns>Kategoriler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Kategoriler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Kategoriler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Kategoriler tablosu Bilgilerini Görüntüle", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesCategoriesQueryResponse>>> GetAllDropboxesCategories([FromQuery] GetAllDropboxesCategoriesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesCategoriesQueryRequest, GetAllDropboxesCategoriesQueryResponse>(request); 
    }

    /// <summary>
    /// Yeni bir Kategoriler tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Kategoriler tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Kategoriler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Kategoriler tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Kategoriler tablosu Eklemek", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateCategoriesCommandResponse>>> Createcategories([FromBody] CreateCategoriesCommandRequest request)
    {
      return await SendCommand<CreateCategoriesCommandRequest, CreateCategoriesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Kategoriler tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Kategoriler tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Kategoriler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Kategoriler tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Kategoriler tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Kategoriler tablosu Güncelemek", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateCategoriesCommandResponse>>> Updatecategories([FromBody] UpdateCategoriesCommandRequest request)
    {
      return await SendCommand<UpdateCategoriesCommandRequest, UpdateCategoriesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Kategoriler tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Kategoriler tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Kategoriler tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Kategoriler tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Kategoriler tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Kategoriler tablosu Silme", Menu = "Kategoriler tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteCategoriesCommandResponse>>> Deletecategories([FromRoute] DeleteCategoriesCommandRequest request)
    {
      return await SendCommand<DeleteCategoriesCommandRequest, DeleteCategoriesCommandResponse>(request);
    }
  }
}
