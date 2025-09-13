using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Files;
using GuidePlatform.Application.Features.Commands.Files.CreateFiles;
using GuidePlatform.Application.Features.Commands.Files.DeleteFiles;
using GuidePlatform.Application.Features.Commands.Files.UpdateFiles;
using GuidePlatform.Application.Features.Queries.Files.GetAllFiles;
using GuidePlatform.Application.Features.Queries.Files.GetFilesById;
using GuidePlatform.Application.Features.Queries.Files.GetAllDropboxesFiles;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class FilesController : BaseController
  {
    public FilesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Files Dosyalar tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Files Dosyalar tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Files Dosyalar tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>Files Dosyalar tablosu listesini döndürür.</returns>
    /// <response code="200">Files Dosyalar tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Files Dosyalar tablosu Listesi Getirir", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllFilesQueryResponse>>> GetAllFiles([FromQuery] GetAllFilesQueryRequest request)
    {
      return await SendQuery<GetAllFilesQueryRequest, GetAllFilesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Files Dosyalar tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Files Dosyalar tablosu kimliğine göre Files Dosyalar tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Files Dosyalar tablosu kimliğini içeren istek.</param>
    /// <returns>Files Dosyalar tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Files Dosyalar tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Files Dosyalar tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Files Dosyalar tablosu Bilgilerini Görüntüle", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetFilesByIdQueryResponse>>> GetByIdFiles([FromQuery] GetFilesByIdQueryRequest request)
    {
      return await SendQuery<GetFilesByIdQueryRequest, GetFilesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Files Dosyalar tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Files Dosyalar tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Files Dosyalar tablosu bilgilerini içeren istek.</param> 
    /// <returns>Files Dosyalar tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Files Dosyalar tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Files Dosyalar tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Files Dosyalar tablosu Bilgilerini Görüntüle", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesFilesQueryResponse>>> GetAllDropboxesFiles([FromQuery] GetAllDropboxesFilesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesFilesQueryRequest, GetAllDropboxesFilesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Files Dosyalar tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Files Dosyalar tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni Files Dosyalar tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Files Dosyalar tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Files Dosyalar tablosu Eklemek", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateFilesCommandResponse>>> CreateFiles([FromBody] CreateFilesCommandRequest request)
    {
      return await SendCommand<CreateFilesCommandRequest, CreateFilesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Files Dosyalar tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Files Dosyalar tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Files Dosyalar tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Files Dosyalar tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Files Dosyalar tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Files Dosyalar tablosu Güncelemek", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateFilesCommandResponse>>> UpdateFiles([FromBody] UpdateFilesCommandRequest request)
    {
      return await SendCommand<UpdateFilesCommandRequest, UpdateFilesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Files Dosyalar tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Files Dosyalar tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Files Dosyalar tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Files Dosyalar tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Files Dosyalar tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Files Dosyalar tablosu Silme", Menu = "Files Dosyalar tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteFilesCommandResponse>>> DeleteFiles([FromRoute] DeleteFilesCommandRequest request)
    {
      return await SendCommand<DeleteFilesCommandRequest, DeleteFilesCommandResponse>(request);
    }
  }
}
