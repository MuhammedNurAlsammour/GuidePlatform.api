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
    /// Admin Ana Ekran Files Dosyalar Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Files Dosyalarlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Files Dosyalarleri getirme parametrelerini içeren istek.</param>
    /// <returns>Files Dosyalar listesini döndürür.</returns>
    /// <response code="200">Files Dosyalar listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Files Dosyalar Listesi Getirir", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<GetAllFilesQueryResponse>>> GetAllFiles([FromQuery] GetAllFilesQueryRequest request)
    {
      return await SendQuery<GetAllFilesQueryRequest, GetAllFilesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Files Dosyalar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Files Dosyalar kimliğine göre Files Dosyalar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Files Dosyalar kimliğini içeren istek.</param>
    /// <returns>Files Dosyalar bilgilerini döndürür.</returns>
    /// <response code="200">Files Dosyalar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Files Dosyalar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Files Dosyalar Bilgilerini Görüntüle", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<GetFilesByIdQueryResponse>>> GetByIdFiles([FromQuery] GetFilesByIdQueryRequest request)
    {
      return await SendQuery<GetFilesByIdQueryRequest, GetFilesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Files Dosyalar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Files Dosyalar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Files Dosyalar bilgilerini içeren istek.</param> 
    /// <returns>Files Dosyalar bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Files Dosyalar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Files Dosyalar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Files Dosyalar Bilgilerini Görüntüle", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesFilesQueryResponse>>> GetAllDropboxesFiles([FromQuery] GetAllDropboxesFilesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesFilesQueryRequest, GetAllDropboxesFilesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Files Dosyalar ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Files Dosyalar ekler.
    /// </remarks>
    /// <param name="request">Yeni Files Dosyalar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Files Dosyalar başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Files Dosyalar Eklemek", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<CreateFilesCommandResponse>>> CreateFiles([FromBody] CreateFilesCommandRequest request)
    {
      return await SendCommand<CreateFilesCommandRequest, CreateFilesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Files Dosyalar kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Files Dosyalarnin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Files Dosyalar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Files Dosyalar başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Files Dosyalar bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Files Dosyalar Güncelemek", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<UpdateFilesCommandResponse>>> UpdateFiles([FromBody] UpdateFilesCommandRequest request)
    {
      return await SendCommand<UpdateFilesCommandRequest, UpdateFilesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Files Dosyalar kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Files Dosyalar kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Files Dosyalar kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Files Dosyalar başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Files Dosyalar bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Files Dosyalar Silme", Menu = "Files-Dosyalar")]
    public async Task<ActionResult<TransactionResultPack<DeleteFilesCommandResponse>>> DeleteFiles([FromRoute] DeleteFilesCommandRequest request)
    {
      return await SendCommand<DeleteFilesCommandRequest, DeleteFilesCommandResponse>(request);
    }
  }
}
