using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.UserFavorites;
using GuidePlatform.Application.Features.Commands.UserFavorites.CreateUserFavorites;
using GuidePlatform.Application.Features.Commands.UserFavorites.DeleteUserFavorites;
using GuidePlatform.Application.Features.Commands.UserFavorites.UpdateUserFavorites;
using GuidePlatform.Application.Features.Queries.UserFavorites.GetAllUserFavorites;
using GuidePlatform.Application.Features.Queries.UserFavorites.GetUserFavoritesById;
using GuidePlatform.Application.Features.Queries.UserFavorites.GetAllDropboxesUserFavorites;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class UserFavoritesController : BaseController
  {
    public UserFavoritesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran UserFavorites Kullanıcı Favorileri Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm UserFavorites Kullanıcı Favorilerilerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm UserFavorites Kullanıcı Favorilerileri getirme parametrelerini içeren istek.</param>
    /// <returns>UserFavorites Kullanıcı Favorileri listesini döndürür.</returns>
    /// <response code="200">UserFavorites Kullanıcı Favorileri listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "UserFavorites Kullanıcı Favorileri Listesi Getirir", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<GetAllUserFavoritesQueryResponse>>> GetAllUserFavorites([FromQuery] GetAllUserFavoritesQueryRequest request)
    {
      return await SendQuery<GetAllUserFavoritesQueryRequest, GetAllUserFavoritesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre UserFavorites Kullanıcı Favorileri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir UserFavorites Kullanıcı Favorileri kimliğine göre UserFavorites Kullanıcı Favorileri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">UserFavorites Kullanıcı Favorileri kimliğini içeren istek.</param>
    /// <returns>UserFavorites Kullanıcı Favorileri bilgilerini döndürür.</returns>
    /// <response code="200">UserFavorites Kullanıcı Favorileri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserFavorites Kullanıcı Favorileri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre UserFavorites Kullanıcı Favorileri Bilgilerini Görüntüle", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<GetUserFavoritesByIdQueryResponse>>> GetByIdUserFavorites([FromQuery] GetUserFavoritesByIdQueryRequest request)
    {
      return await SendQuery<GetUserFavoritesByIdQueryRequest, GetUserFavoritesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes UserFavorites Kullanıcı Favorileri bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes UserFavorites Kullanıcı Favorileri bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes UserFavorites Kullanıcı Favorileri bilgilerini içeren istek.</param> 
    /// <returns>UserFavorites Kullanıcı Favorileri bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes UserFavorites Kullanıcı Favorileri bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">UserFavorites Kullanıcı Favorileri bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes UserFavorites Kullanıcı Favorileri Bilgilerini Görüntüle", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesUserFavoritesQueryResponse>>> GetAllDropboxesUserFavorites([FromQuery] GetAllDropboxesUserFavoritesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesUserFavoritesQueryRequest, GetAllDropboxesUserFavoritesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir UserFavorites Kullanıcı Favorileri ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir UserFavorites Kullanıcı Favorileri ekler.
    /// </remarks>
    /// <param name="request">Yeni UserFavorites Kullanıcı Favorileri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">UserFavorites Kullanıcı Favorileri başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "UserFavorites Kullanıcı Favorileri Eklemek", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<CreateUserFavoritesCommandResponse>>> CreateUserFavorites([FromBody] CreateUserFavoritesCommandRequest request)
    {
      return await SendCommand<CreateUserFavoritesCommandRequest, CreateUserFavoritesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir UserFavorites Kullanıcı Favorileri kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserFavorites Kullanıcı Favorilerinin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek UserFavorites Kullanıcı Favorileri bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserFavorites Kullanıcı Favorileri başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek UserFavorites Kullanıcı Favorileri bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "UserFavorites Kullanıcı Favorileri Güncelemek", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<UpdateUserFavoritesCommandResponse>>> UpdateUserFavorites([FromBody] UpdateUserFavoritesCommandRequest request)
    {
      return await SendCommand<UpdateUserFavoritesCommandRequest, UpdateUserFavoritesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip UserFavorites Kullanıcı Favorileri kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip UserFavorites Kullanıcı Favorileri kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek UserFavorites Kullanıcı Favorileri kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">UserFavorites Kullanıcı Favorileri başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek UserFavorites Kullanıcı Favorileri bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "UserFavorites Kullanıcı Favorileri Silme", Menu = "UserFavorites-Kullanıcı Favorileri")]
    public async Task<ActionResult<TransactionResultPack<DeleteUserFavoritesCommandResponse>>> DeleteUserFavorites([FromRoute] DeleteUserFavoritesCommandRequest request)
    {
      return await SendCommand<DeleteUserFavoritesCommandRequest, DeleteUserFavoritesCommandResponse>(request);
    }
  }
}
