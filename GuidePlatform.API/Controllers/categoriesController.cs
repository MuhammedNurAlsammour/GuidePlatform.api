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
    /// Admin Ana Ekran Kategoriler Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Kategorilerlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Kategorilerleri getirme parametrelerini içeren istek.</param>
    /// <returns>Kategoriler listesini döndürür.</returns>
    /// <response code="200">Kategoriler listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Kategoriler Listesi Getirir", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<GetAllCategoriesQueryResponse>>> GetAllcategories([FromQuery] GetAllCategoriesQueryRequest request)
    {
      return await SendQuery<GetAllCategoriesQueryRequest, GetAllCategoriesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Kategoriler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Kategoriler kimliğine göre Kategoriler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Kategoriler kimliğini içeren istek.</param>
    /// <returns>Kategoriler bilgilerini döndürür.</returns>
    /// <response code="200">Kategoriler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Kategoriler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Kategoriler Bilgilerini Görüntüle", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<GetCategoriesByIdQueryResponse>>> GetByIdcategories([FromQuery] GetCategoriesByIdQueryRequest request)
    {
      return await SendQuery<GetCategoriesByIdQueryRequest, GetCategoriesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Kategoriler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Kategoriler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Kategoriler bilgilerini içeren istek.</param>
    /// <returns>Kategoriler bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Kategoriler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Kategoriler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Kategoriler Bilgilerini Görüntüle", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesCategoriesQueryResponse>>> GetAllDropboxesCategories([FromQuery] GetAllDropboxesCategoriesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesCategoriesQueryRequest, GetAllDropboxesCategoriesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Kategoriler ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Kategoriler ekler.
    /// </remarks>
    /// <param name="request">Yeni Kategoriler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Kategoriler başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Kategoriler Eklemek", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<CreateCategoriesCommandResponse>>> Createcategories([FromBody] CreateCategoriesCommandRequest request)
    {
      return await SendCommand<CreateCategoriesCommandRequest, CreateCategoriesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Kategoriler kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Kategorilernin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Kategoriler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Kategoriler başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Kategoriler bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Kategoriler Güncelemek", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<UpdateCategoriesCommandResponse>>> Updatecategories([FromBody] UpdateCategoriesCommandRequest request)
    {
      return await SendCommand<UpdateCategoriesCommandRequest, UpdateCategoriesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Kategoriler kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Kategoriler kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Kategoriler kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Kategoriler başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Kategoriler bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Kategoriler Silme", Menu = "Kategoriler")]
    public async Task<ActionResult<TransactionResultPack<DeleteCategoriesCommandResponse>>> Deletecategories([FromRoute] DeleteCategoriesCommandRequest request)
    {
      return await SendCommand<DeleteCategoriesCommandRequest, DeleteCategoriesCommandResponse>(request);
    }
  }
}
