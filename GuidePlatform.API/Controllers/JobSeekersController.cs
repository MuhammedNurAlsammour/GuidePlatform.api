using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobSeekers;
using GuidePlatform.Application.Features.Commands.JobSeekers.CreateJobSeekers;
using GuidePlatform.Application.Features.Commands.JobSeekers.DeleteJobSeekers;
using GuidePlatform.Application.Features.Commands.JobSeekers.UpdateJobSeekers;
using GuidePlatform.Application.Features.Commands.JobSeekers.UpdateJobSeekerStatus;
using GuidePlatform.Application.Features.Commands.JobSeekers.BulkApproveJobSeekers;
using GuidePlatform.Application.Features.Queries.JobSeekers.GetAllJobSeekers;
using GuidePlatform.Application.Features.Queries.JobSeekers.GetJobSeekersById;
using GuidePlatform.Application.Features.Queries.JobSeekers.GetAllDropboxesJobSeekers;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class JobSeekersController : BaseController
  {
    public JobSeekersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran JobSeekers İş Arayanları tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm JobSeekers İş Arayanları tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm JobSeekers İş Arayanları tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>JobSeekers İş Arayanları tablosu listesini döndürür.</returns>
    /// <response code="200">JobSeekers İş Arayanları tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "JobSeekers İş Arayanları tablosu Listesi Getirir", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllJobSeekersQueryResponse>>> GetAllJobSeekers([FromQuery] GetAllJobSeekersQueryRequest request)
    {
      return await SendQuery<GetAllJobSeekersQueryRequest, GetAllJobSeekersQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre JobSeekers İş Arayanları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir JobSeekers İş Arayanları tablosu kimliğine göre JobSeekers İş Arayanları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">JobSeekers İş Arayanları tablosu kimliğini içeren istek.</param>
    /// <returns>JobSeekers İş Arayanları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">JobSeekers İş Arayanları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">JobSeekers İş Arayanları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre JobSeekers İş Arayanları tablosu Bilgilerini Görüntüle", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetJobSeekersByIdQueryResponse>>> GetByIdJobSeekers([FromQuery] GetJobSeekersByIdQueryRequest request)
    {
      return await SendQuery<GetJobSeekersByIdQueryRequest, GetJobSeekersByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes JobSeekers İş Arayanları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes JobSeekers İş Arayanları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes JobSeekers İş Arayanları tablosu bilgilerini içeren istek.</param> 
    /// <returns>JobSeekers İş Arayanları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes JobSeekers İş Arayanları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">JobSeekers İş Arayanları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes JobSeekers İş Arayanları tablosu Bilgilerini Görüntüle", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesJobSeekersQueryResponse>>> GetAllDropboxesJobSeekers([FromQuery] GetAllDropboxesJobSeekersQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesJobSeekersQueryRequest, GetAllDropboxesJobSeekersQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir JobSeekers İş Arayanları tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir JobSeekers İş Arayanları tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni JobSeekers İş Arayanları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">JobSeekers İş Arayanları tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "JobSeekers İş Arayanları tablosu Eklemek", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateJobSeekersCommandResponse>>> CreateJobSeekers([FromBody] CreateJobSeekersCommandRequest request)
    {
      return await SendCommand<CreateJobSeekersCommandRequest, CreateJobSeekersCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir JobSeekers İş Arayanları tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobSeekers İş Arayanları tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek JobSeekers İş Arayanları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobSeekers İş Arayanları tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek JobSeekers İş Arayanları tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobSeekers İş Arayanları tablosu Güncelemek", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateJobSeekersCommandResponse>>> UpdateJobSeekers([FromBody] UpdateJobSeekersCommandRequest request)
    {
      return await SendCommand<UpdateJobSeekersCommandRequest, UpdateJobSeekersCommandResponse>(request);
    }

    /// <summary>
    /// JobSeeker durumunu günceller (Approve/Deny/Expire vb.)
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobSeeker'ın durumunu günceller.
    /// </remarks>
    /// <param name="request">0=Beklemede, 1=Aktif, 2=Onaylandı, 3=Reddedildi, 4=Süresi Doldu, 5=Pasif, 6=Silindi enum</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobSeeker durumu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek JobSeeker bulunamazsa.</response>
    [HttpPatch("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobSeeker Durumu Güncelleme Onaylandı/Reddedildi/Süresi Doldu/Pasif/Silindi", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateJobSeekerStatusCommandResponse>>> UpdateJobSeekerStatus([FromBody] UpdateJobSeekerStatusCommandRequest request)
    {
      return await SendCommand<UpdateJobSeekerStatusCommandRequest, UpdateJobSeekerStatusCommandResponse>(request);
    }

    /// <summary>
    /// Birden fazla JobSeeker'ı toplu olarak onaylar/reddeder (Bulk Approve/Deny)
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, birden fazla JobSeeker'ın durumunu toplu olarak günceller.
    /// </remarks>
    /// <param name="request">JobSeeker ID'leri listesi ve yeni durum bilgilerini içeren istek.</param>
    /// <param name="request">0=Beklemede, 1=Aktif, 2=Onaylandı, 3=Reddedildi, 4=Süresi Doldu, 5=Pasif, 6=Silindi enum</param>
    /// <returns>Toplu işlem sonucunu döndürür.</returns>
    /// <response code="200">Toplu işlem başarıyla tamamlandı.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPatch("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobSeeker Toplu Onay/Red İşlemi", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<BulkApproveJobSeekersCommandResponse>>> BulkApproveJobSeekers([FromBody] BulkApproveJobSeekersCommandRequest request)
    {
      return await SendCommand<BulkApproveJobSeekersCommandRequest, BulkApproveJobSeekersCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip JobSeekers İş Arayanları tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobSeekers İş Arayanları tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek JobSeekers İş Arayanları tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobSeekers İş Arayanları tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek JobSeekers İş Arayanları tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "JobSeekers İş Arayanları tablosu Silme", Menu = "JobSeekers İş Arayanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteJobSeekersCommandResponse>>> DeleteJobSeekers([FromRoute] DeleteJobSeekersCommandRequest request)
    {
      return await SendCommand<DeleteJobSeekersCommandRequest, DeleteJobSeekersCommandResponse>(request);
    }
  }
}
