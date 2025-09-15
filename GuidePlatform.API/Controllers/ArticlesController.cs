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
    /// Admin Ana Ekran Articles Makaleler Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Articles Makalelerlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Articles Makalelerleri getirme parametrelerini içeren istek.</param>
    /// <returns>Articles Makaleler listesini döndürür.</returns>
    /// <response code="200">Articles Makaleler listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Articles Makaleler Listesi Getirir", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<GetAllArticlesQueryResponse>>> GetAllArticles([FromQuery] GetAllArticlesQueryRequest request)
    {
      return await SendQuery<GetAllArticlesQueryRequest, GetAllArticlesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Articles Makaleler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Articles Makaleler kimliğine göre Articles Makaleler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Articles Makaleler kimliğini içeren istek.</param>
    /// <returns>Articles Makaleler bilgilerini döndürür.</returns>
    /// <response code="200">Articles Makaleler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Articles Makaleler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Articles Makaleler Bilgilerini Görüntüle", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<GetArticlesByIdQueryResponse>>> GetByIdArticles([FromQuery] GetArticlesByIdQueryRequest request)
    {
      return await SendQuery<GetArticlesByIdQueryRequest, GetArticlesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Articles Makaleler bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Articles Makaleler bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Articles Makaleler bilgilerini içeren istek.</param> 
    /// <returns>Articles Makaleler bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Articles Makaleler bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Articles Makaleler bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Articles Makaleler Bilgilerini Görüntüle", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesArticlesQueryResponse>>> GetAllDropboxesArticles([FromQuery] GetAllDropboxesArticlesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesArticlesQueryRequest, GetAllDropboxesArticlesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Articles Makaleler ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Articles Makaleler ekler.
    /// </remarks>
    /// <param name="request">Yeni Articles Makaleler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Articles Makaleler başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Articles Makaleler Eklemek", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<CreateArticlesCommandResponse>>> CreateArticles([FromBody] CreateArticlesCommandRequest request)
    {
      return await SendCommand<CreateArticlesCommandRequest, CreateArticlesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Articles Makaleler kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Articles Makalelernin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Articles Makaleler bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Articles Makaleler başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Articles Makaleler bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Articles Makaleler Güncelemek", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<UpdateArticlesCommandResponse>>> UpdateArticles([FromBody] UpdateArticlesCommandRequest request)
    {
      return await SendCommand<UpdateArticlesCommandRequest, UpdateArticlesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Articles Makaleler kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Articles Makaleler kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Articles Makaleler kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Articles Makaleler başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Articles Makaleler bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Articles Makaleler Silme", Menu = "Articles-Makaleler")]
    public async Task<ActionResult<TransactionResultPack<DeleteArticlesCommandResponse>>> DeleteArticles([FromRoute] DeleteArticlesCommandRequest request)
    {
      return await SendCommand<DeleteArticlesCommandRequest, DeleteArticlesCommandResponse>(request);
    }
  }
}
