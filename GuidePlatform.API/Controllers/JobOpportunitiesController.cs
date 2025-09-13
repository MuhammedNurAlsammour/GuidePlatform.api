using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.JobOpportunities;
using GuidePlatform.Application.Features.Commands.JobOpportunities.CreateJobOpportunities;
using GuidePlatform.Application.Features.Commands.JobOpportunities.DeleteJobOpportunities;
using GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunities;
using GuidePlatform.Application.Features.Commands.JobOpportunities.UpdateJobOpportunityStatus;
using GuidePlatform.Application.Features.Commands.JobOpportunities.BulkApproveJobOpportunities;
using GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllJobOpportunities;
using GuidePlatform.Application.Features.Queries.JobOpportunities.GetJobOpportunitiesById;
using GuidePlatform.Application.Features.Queries.JobOpportunities.GetAllDropboxesJobOpportunities;
using GuidePlatform.Application.Features.Queries.Base;
using GuidePlatform.API.Controllers.Base;
using GuidePlatform.Domain.Enums;

using System.Net;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class JobOpportunitiesController : BaseController
  {
    public JobOpportunitiesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran JobOpportunities İş İlanları tablosu Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm JobOpportunities İş İlanları tablosulerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm JobOpportunities İş İlanları tablosuleri getirme parametrelerini içeren istek.</param>
    /// <returns>JobOpportunities İş İlanları tablosu listesini döndürür.</returns>
    /// <response code="200">JobOpportunities İş İlanları tablosu listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "JobOpportunities İş İlanları tablosu Listesi Getirir", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllJobOpportunitiesQueryResponse>>> GetAllJobOpportunities([FromQuery] GetAllJobOpportunitiesQueryRequest request)
    {
      return await SendQuery<GetAllJobOpportunitiesQueryRequest, GetAllJobOpportunitiesQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre JobOpportunities İş İlanları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir JobOpportunities İş İlanları tablosu kimliğine göre JobOpportunities İş İlanları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">JobOpportunities İş İlanları tablosu kimliğini içeren istek.</param>
    /// <returns>JobOpportunities İş İlanları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">JobOpportunities İş İlanları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">JobOpportunities İş İlanları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre JobOpportunities İş İlanları tablosu Bilgilerini Görüntüle", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetJobOpportunitiesByIdQueryResponse>>> GetByIdJobOpportunities([FromQuery] GetJobOpportunitiesByIdQueryRequest request)
    {
      return await SendQuery<GetJobOpportunitiesByIdQueryRequest, GetJobOpportunitiesByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes JobOpportunities İş İlanları tablosu bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes JobOpportunities İş İlanları tablosu bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes JobOpportunities İş İlanları tablosu bilgilerini içeren istek.</param> 
    /// <returns>JobOpportunities İş İlanları tablosu bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes JobOpportunities İş İlanları tablosu bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">JobOpportunities İş İlanları tablosu bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes JobOpportunities İş İlanları tablosu Bilgilerini Görüntüle", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesJobOpportunitiesQueryResponse>>> GetAllDropboxesJobOpportunities([FromQuery] GetAllDropboxesJobOpportunitiesQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesJobOpportunitiesQueryRequest, GetAllDropboxesJobOpportunitiesQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir JobOpportunities İş İlanları tablosu ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir JobOpportunities İş İlanları tablosu ekler.
    /// </remarks>
    /// <param name="request">Yeni JobOpportunities İş İlanları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">JobOpportunities İş İlanları tablosu başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "JobOpportunities İş İlanları tablosu Eklemek", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<CreateJobOpportunitiesCommandResponse>>> CreateJobOpportunities([FromBody] CreateJobOpportunitiesCommandRequest request)
    {
      return await SendCommand<CreateJobOpportunitiesCommandRequest, CreateJobOpportunitiesCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir JobOpportunities İş İlanları tablosu kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobOpportunities İş İlanları tablosunin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek JobOpportunities İş İlanları tablosu bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobOpportunities İş İlanları tablosu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek JobOpportunities İş İlanları tablosu bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobOpportunities İş İlanları tablosu Güncelemek", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateJobOpportunitiesCommandResponse>>> UpdateJobOpportunities([FromBody] UpdateJobOpportunitiesCommandRequest request)
    {
      return await SendCommand<UpdateJobOpportunitiesCommandRequest, UpdateJobOpportunitiesCommandResponse>(request);
    }

    /// <summary>
    /// JobOpportunity durumunu günceller (Approve/Deny/Expire vb.)
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobOpportunity'nin durumunu günceller.
    /// </remarks>
    /// <param name="request">0=Beklemede, 1=Aktif, 2=Onaylandı, 3=Reddedildi, 4=Süresi Doldu, 5=Pasif, 6=Silindi enum</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobOpportunity durumu başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek JobOpportunity bulunamazsa.</response>
    [HttpPatch("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobOpportunity Durumu Güncelleme Onaylandı/Reddedildi/Süresi Doldu/Pasif/Silindi", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<UpdateJobOpportunityStatusCommandResponse>>> UpdateJobOpportunityStatus([FromBody] UpdateJobOpportunityStatusCommandRequest request)
    {
      return await SendCommand<UpdateJobOpportunityStatusCommandRequest, UpdateJobOpportunityStatusCommandResponse>(request);
    }

    /// <summary>
    /// Birden fazla JobOpportunity'yi toplu olarak onaylar/reddeder (Bulk Approve/Deny)
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, birden fazla JobOpportunity'nin durumunu toplu olarak günceller.
    /// </remarks>
    /// <param name="request">JobOpportunity ID'leri listesi ve yeni durum bilgilerini içeren istek.</param>
    /// <param name="request">0=Beklemede, 1=Aktif, 2=Onaylandı, 3=Reddedildi, 4=Süresi Doldu, 5=Pasif, 6=Silindi enum</param>
    /// <returns>Toplu işlem sonucunu döndürür.</returns>
    /// <response code="200">Toplu işlem başarıyla tamamlandı.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPatch("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "JobOpportunity Toplu Onay/Red İşlemi", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<BulkApproveJobOpportunitiesCommandResponse>>> BulkApproveJobOpportunities([FromBody] BulkApproveJobOpportunitiesCommandRequest request)
    {
      return await SendCommand<BulkApproveJobOpportunitiesCommandRequest, BulkApproveJobOpportunitiesCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip JobOpportunities İş İlanları tablosu kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip JobOpportunities İş İlanları tablosu kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek JobOpportunities İş İlanları tablosu kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">JobOpportunities İş İlanları tablosu başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek JobOpportunities İş İlanları tablosu bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "JobOpportunities İş İlanları tablosu Silme", Menu = "JobOpportunities İş İlanları tablosu")]
    public async Task<ActionResult<TransactionResultPack<DeleteJobOpportunitiesCommandResponse>>> DeleteJobOpportunities([FromRoute] DeleteJobOpportunitiesCommandRequest request)
    {
      return await SendCommand<DeleteJobOpportunitiesCommandRequest, DeleteJobOpportunitiesCommandResponse>(request);
    }
  }
}
