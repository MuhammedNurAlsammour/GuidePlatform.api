using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.BusinessReviews;
using GuidePlatform.Application.Dtos.Enums;
using GuidePlatform.Application.Features.Commands.BusinessReviews.CreateBusinessReviews;
using GuidePlatform.Application.Features.Commands.BusinessReviews.DeleteBusinessReviews;
using GuidePlatform.Application.Features.Commands.BusinessReviews.UpdateBusinessReviews;
using GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllBusinessReviews;
using GuidePlatform.Application.Features.Queries.BusinessReviews.GetBusinessReviewsById;
using GuidePlatform.Application.Features.Queries.BusinessReviews.GetAllDropboxesBusinessReviews;
using GuidePlatform.Application.Features.Queries.BusinessReviews.GetODataBusinessReviews;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;
using GuidePlatform.Application.Abstractions.Contexts;
using Microsoft.EntityFrameworkCore;
using Karmed.External.Auth.Library.Contexts;
using GuidePlatform.Application.Features.Queries.BusinessReviews.GetODataBusinessReviews;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BusinessReviewsController : BaseController
  {
    private readonly IApplicationDbContext _context;

    public BusinessReviewsController(IMediator mediator, IApplicationDbContext context) : base(mediator)
    {
      _context = context;
    }

    /// <summary>
    /// OData endpoint - BusinessReviews için gelişmiş filtreleme ve sorgulama
    /// </summary>
    /// <remarks>
    /// Bu endpoint OData sorgularını destekler:
    /// - $filter: BusinessId eq 'guid', Rating ge 4, IsApproved eq true
    /// - $orderby: Rating desc, RowCreatedDate desc
    /// - $select: Id, BusinessId, Rating, Comment
    /// - $top: 10, $skip: 20
    /// - $count: true
    /// </remarks>
    /// <param name="queryOptions">OData query options</param>
    /// <returns>BusinessReviews listesi</returns>
    /// <response code="200">BusinessReviews listesi döndürür.</response>
    /// <response code="400">OData sorgusu geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("odata")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "OData BusinessReviews Sorgulama", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetODataBusinessReviewsQueryResponse>>> GetODataBusinessReviews(
      [FromQuery(Name = "$filter")] string? filter = null,
      [FromQuery(Name = "$orderby")] string? orderby = null,
      [FromQuery(Name = "$select")] string? select = null,
      [FromQuery(Name = "$top")] int? top = null,
      [FromQuery(Name = "$skip")] int? skip = null,
      [FromQuery(Name = "$count")] bool? count = null)
    {
      var request = new GetODataBusinessReviewsQueryRequest
      {
        Filter = filter,
        OrderBy = orderby,
        Select = select,
        Top = top,
        Skip = skip,
        Count = count
      };

      var response = await _mediator.Send(request);
      return response;
    }


    /// <summary>
    /// Admin Ana Ekran BusinessReviews İşletme Değerlendirmeleri tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm BusinessReviews İşletme Değerlendirmeleri tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm BusinessReviews İşletme Değerlendirmeleri tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>BusinessReviews İşletme Değerlendirmeleri tablosu listesini döndürür.</returns>
    /// <response code="200">BusinessReviews İşletme Değerlendirmeleri tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "BusinessReviews İşletme Değerlendirmeleri tablosu Listesi Getirir", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllBusinessReviewsQueryResponse>>> GetAllBusinessReviews([FromQuery] GetAllBusinessReviewsQueryRequest request)
    {
      return await SendQuery<GetAllBusinessReviewsQueryRequest, GetAllBusinessReviewsQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir BusinessReviews İşletme Değerlendirmeleri tablosu kimliğine göre BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">BusinessReviews İşletme Değerlendirmeleri tablosu kimliğini içeren istek.</param>
    /// <returns>BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessReviews İşletme Değerlendirmeleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre BusinessReviews İşletme Değerlendirmeleri tablosu Bilgilerini Görüntüle", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetBusinessReviewsByIdQueryResponse>>> GetByIdBusinessReviews([FromQuery] GetBusinessReviewsByIdQueryRequest request)
    {
      return await SendQuery<GetBusinessReviewsByIdQueryRequest, GetBusinessReviewsByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini içeren istek.</param> 
    /// <returns>BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">BusinessReviews İşletme Değerlendirmeleri tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes BusinessReviews İşletme Değerlendirmeleri tablosu Bilgilerini Görüntüle", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBusinessReviewsQueryResponse>>> GetAllDropboxesBusinessReviews([FromQuery] GetAllDropboxesBusinessReviewsQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBusinessReviewsQueryRequest, GetAllDropboxesBusinessReviewsQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir BusinessReviews İşletme Değerlendirmeleri tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir BusinessReviews İşletme Değerlendirmeleri tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">BusinessReviews İşletme Değerlendirmeleri tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "BusinessReviews İşletme Değerlendirmeleri tablosu Eklemek", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateBusinessReviewsCommandResponse>>> CreateBusinessReviews([FromBody] CreateBusinessReviewsCommandRequest request)
    {
      return await SendCommand<CreateBusinessReviewsCommandRequest, CreateBusinessReviewsCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir BusinessReviews İşletme Değerlendirmeleri tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessReviews İşletme Değerlendirmeleri tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek BusinessReviews İşletme Değerlendirmeleri tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessReviews İşletme Değerlendirmeleri tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek BusinessReviews İşletme Değerlendirmeleri tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "BusinessReviews İşletme Değerlendirmeleri tablosu Güncelemek", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateBusinessReviewsCommandResponse>>> UpdateBusinessReviews([FromBody] UpdateBusinessReviewsCommandRequest request)
    {
      return await SendCommand<UpdateBusinessReviewsCommandRequest, UpdateBusinessReviewsCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip BusinessReviews İşletme Değerlendirmeleri tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip BusinessReviews İşletme Değerlendirmeleri tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek BusinessReviews İşletme Değerlendirmeleri tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">BusinessReviews İşletme Değerlendirmeleri tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek BusinessReviews İşletme Değerlendirmeleri tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "BusinessReviews İşletme Değerlendirmeleri tablosu Silme", Menu = "BusinessReviews İşletme Değerlendirmeleri tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteBusinessReviewsCommandResponse>>> DeleteBusinessReviews([FromRoute] DeleteBusinessReviewsCommandRequest request)
    {
      return await SendCommand<DeleteBusinessReviewsCommandRequest, DeleteBusinessReviewsCommandResponse>(request);
    }
  }
}
