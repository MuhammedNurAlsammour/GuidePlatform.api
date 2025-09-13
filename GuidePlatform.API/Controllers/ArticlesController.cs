using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Articles;
using GuidePlatform.Application.Features.Commands.Articles.CreateArticles;
using GuidePlatform.Application.Features.Commands.Articles.DeleteArticles;
using GuidePlatform.Application.Features.Commands.Articles.UpdateArticles;
using GuidePlatform.Application.Features.Queries.Articles.GetAllArticles;
using GuidePlatform.Application.Features.Queries.Articles.GetArticlesById;
using GuidePlatform.Application.Features.Queries.Articles.GetAllDropboxesArticles;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class ArticlesController : BaseController
  {
    public ArticlesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Articles Makaleler tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Articles Makaleler tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Articles Makaleler tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Articles Makaleler tablosu listesini döndürür.</returns>
    /// <response code="200">Articles Makaleler tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Articles Makaleler tablosu Listesi Getirir", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllArticlesQueryResponse>>> GetAllArticles([FromQuery] GetAllArticlesQueryRequest request)
    {
      return await SendQuery<GetAllArticlesQueryRequest, GetAllArticlesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Articles Makaleler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Articles Makaleler tablosu kimliğine göre Articles Makaleler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Articles Makaleler tablosu kimliğini içeren istek.</param>
    /// <returns>Articles Makaleler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Articles Makaleler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Articles Makaleler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Articles Makaleler tablosu Bilgilerini Görüntüle", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetArticlesByIdQueryResponse>>> GetByIdArticles([FromQuery] GetArticlesByIdQueryRequest request)
    {
      return await SendQuery<GetArticlesByIdQueryRequest, GetArticlesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Articles Makaleler tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Articles Makaleler tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Articles Makaleler tablosu bilgilerini içeren istek.</param> 
    /// <returns>Articles Makaleler tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Articles Makaleler tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Articles Makaleler tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Articles Makaleler tablosu Bilgilerini Görüntüle", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesArticlesQueryResponse>>> GetAllDropboxesArticles([FromQuery] GetAllDropboxesArticlesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesArticlesQueryRequest, GetAllDropboxesArticlesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Articles Makaleler tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Articles Makaleler tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Articles Makaleler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Articles Makaleler tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Articles Makaleler tablosu Eklemek", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateArticlesCommandResponse>>> CreateArticles([FromBody] CreateArticlesCommandRequest request)
    {
      return await SendCommand<CreateArticlesCommandRequest, CreateArticlesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Articles Makaleler tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Articles Makaleler tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Articles Makaleler tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Articles Makaleler tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Articles Makaleler tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Articles Makaleler tablosu Güncelemek", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateArticlesCommandResponse>>> UpdateArticles([FromBody] UpdateArticlesCommandRequest request)
    {
      return await SendCommand<UpdateArticlesCommandRequest, UpdateArticlesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Articles Makaleler tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Articles Makaleler tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Articles Makaleler tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Articles Makaleler tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Articles Makaleler tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Articles Makaleler tablosu Silme", Menu = "Articles Makaleler tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteArticlesCommandResponse>>> DeleteArticles([FromRoute] DeleteArticlesCommandRequest request)
    {
      return await SendCommand<DeleteArticlesCommandRequest, DeleteArticlesCommandResponse>(request);
    }
  }
}
