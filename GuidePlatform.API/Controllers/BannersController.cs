using Karmed.External.Auth.Library.CustomAttributes;
using Karmed.External.Auth.Library.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Dtos.ResponseDtos.Banners;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Banners.CreateBanners;
using GuidePlatform.Application.Features.Commands.Banners.DeleteBanners;
using GuidePlatform.Application.Features.Commands.Banners.UpdateBanners;
using GuidePlatform.Application.Features.Commands.Banners.CreateBannerWithImage;
using GuidePlatform.Application.Features.Queries.Banners.GetAllBanners;
using GuidePlatform.Application.Features.Queries.Banners.GetBannersById;
using GuidePlatform.Application.Features.Queries.Banners.GetAllDropboxesBanners;
using GuidePlatform.Application.Features.Queries.Base;
using System.ComponentModel.DataAnnotations;
using GuidePlatform.API.Controllers.Base;

using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace GuidePlatform.API.Controllers
{
  [Route("api/[controller]")]
  [Authorize(AuthenticationSchemes = "Admin")]
  public class BannersController : BaseController
  {
    public BannersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Admin Ana Ekran Banners Bannerlar Listesi Getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen sayfa ve boyuta göre tüm Banners Bannerlarlerin listesini getirir.
    /// </remarks>
    /// <param name="request">Tüm Banners Bannerlarleri getirme parametrelerini içeren istek.</param>
    /// <returns>Banners Bannerlar listesini döndürür.</returns>
    /// <response code="200">Banners Bannerlar listesini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Banners Bannerlar Listesi Getirir", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<GetAllBannersQueryResponse>>> GetAllBanners([FromQuery] GetAllBannersQueryRequest request)
    {
      return await SendQuery<GetAllBannersQueryRequest, GetAllBannersQueryResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye göre Banners Bannerlar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirli bir Banners Bannerlar kimliğine göre Banners Bannerlar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Banners Bannerlar kimliğini içeren istek.</param>
    /// <returns>Banners Bannerlar bilgilerini döndürür.</returns>
    /// <response code="200">Banners Bannerlar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Banners Bannerlar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "ID ye Göre Banners Bannerlar Bilgilerini Görüntüle", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<GetBannersByIdQueryResponse>>> GetByIdBanners([FromQuery] GetBannersByIdQueryRequest request)
    {
      return await SendQuery<GetBannersByIdQueryRequest, GetBannersByIdQueryResponse>(request);
    }

    /// <summary>
    /// Dropboxes Banners Bannerlar bilgilerini getirir.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, Dropboxes Banners Bannerlar bilgilerini getirir.
    /// </remarks>
    /// <param name="request">Dropboxes Banners Bannerlar bilgilerini içeren istek.</param> 
    /// <returns>Banners Bannerlar bilgilerini döndürür.</returns>
    /// <response code="200">Dropboxes Banners Bannerlar bilgilerini döndürür.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Banners Bannerlar bulunamazsa.</response>
    [HttpGet("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Dropboxes Banners Bannerlar Bilgilerini Görüntüle", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<GetAllDropboxesBannersQueryResponse>>> GetAllDropboxesBanners([FromQuery] GetAllDropboxesBannersQueryRequest request)
    {
      return await SendQuery<GetAllDropboxesBannersQueryRequest, GetAllDropboxesBannersQueryResponse>(request);
    }

    /// <summary>
    /// Yeni bir Banners Bannerlar ekler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, yeni bir Banners Bannerlar ekler.
    /// </remarks>
    /// <param name="request">Yeni Banners Bannerlar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Banners Bannerlar başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Banners Bannerlar Eklemek", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<CreateBannersCommandResponse>>> CreateBanners([FromBody] CreateBannersCommandRequest request)
    {
      return await SendCommand<CreateBannersCommandRequest, CreateBannersCommandResponse>(request, HttpStatusCode.Created);
    }

    /// <summary>
    /// Mevcut bir Banners Bannerlar kaydını günceller.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Banners Bannerlarnin bilgilerini günceller.
    /// </remarks>
    /// <param name="request">Güncellenecek Banners Bannerlar bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Banners Bannerlar başarıyla güncellendi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Banners Bannerlar bulunamazsa.</response>
    [HttpPut("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Banners Bannerlar Güncelemek", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<UpdateBannersCommandResponse>>> UpdateBanners([FromBody] UpdateBannersCommandRequest request)
    {
      return await SendCommand<UpdateBannersCommandRequest, UpdateBannersCommandResponse>(request);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip Banners Bannerlar kaydını siler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, belirtilen ID'ye sahip Banners Bannerlar kaydını siler.
    /// </remarks>
    /// <param name="request">Silinecek Banners Bannerlar kimliğini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="200">Banners Bannerlar başarıyla silindi.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Silinecek Banners Bannerlar bulunamazsa.</response>
    [HttpDelete("[action]/{Id}")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Banners Bannerlar Silme", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<DeleteBannersCommandResponse>>> DeleteBanners([FromRoute] DeleteBannersCommandRequest request)
    {
      return await SendCommand<DeleteBannersCommandRequest, DeleteBannersCommandResponse>(request);
    }

    /// <summary>
    /// Tekrarlanan (duplicate) bannerları temizler.
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, aynı başlık ve açıklamaya sahip tekrarlanan bannerları bulur ve en eskiler hariç hepsini siler.
    /// </remarks>
    /// <returns>Temizlenen banner sayısını döndürür.</returns>
    /// <response code="200">Tekrarlanan bannerlar başarıyla temizlendi.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Tekrarlanan Bannerları Temizleme", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<object>>> CleanDuplicateBanners()
    {
      try
      {
        var getAllRequest = new GetAllBannersQueryRequest();
        var allBannersResult = await _mediator.Send(getAllRequest);

        if (!allBannersResult.OperationStatus || allBannersResult.Result?.banners == null)
        {
          return Ok(ResultFactory.CreateSuccessResult<object>(
            new { CleanedCount = 0 },
            null,
            null,
            "Bilgi",
            "Temizlenecek banner bulunamadı.",
            "Herhangi bir tekrarlanan banner bulunamadı."
          ));
        }

        var banners = allBannersResult.Result.banners;
        var duplicateGroups = banners
          .GroupBy(b => new
          {
            Title = b.Title?.Trim().ToLower(),
            Description = b.Description?.Trim().ToLower(),
            AuthUserId = b.AuthUserId,
            AuthCustomerId = b.AuthCustomerId
          })
          .Where(g => g.Count() > 1)
          .ToList();

        int cleanedCount = 0;
        var errors = new List<string>();

        foreach (var group in duplicateGroups)
        {
          // En eski banner'ı koru (RowCreatedDate'e göre), diğerlerini sil
          var bannersToDelete = group.OrderBy(b => b.RowCreatedDate).Skip(1).ToList();

          foreach (var bannerToDelete in bannersToDelete)
          {
            try
            {
              var deleteRequest = new DeleteBannersCommandRequest { Id = bannerToDelete.Id.ToString() };
              var deleteResult = await _mediator.Send(deleteRequest);

              if (deleteResult.OperationStatus)
              {
                cleanedCount++;
              }
              else
              {
                errors.Add($"Banner ID {bannerToDelete.Id} silinemedi: {deleteResult.OperationResult.MessageContent}");
              }
            }
            catch (Exception ex)
            {
              errors.Add($"Banner ID {bannerToDelete.Id} silinirken hata: {ex.Message}");
            }
          }
        }

        return Ok(ResultFactory.CreateSuccessResult<object>(
          new
          {
            CleanedCount = cleanedCount,
            DuplicateGroupsFound = duplicateGroups.Count,
            Errors = errors
          },
          null,
          null,
          "İşlem Tamamlandı",
          $"{cleanedCount} tekrarlanan banner temizlendi.",
          errors.Any()
            ? $"Toplam {cleanedCount} banner silindi. {errors.Count} hata oluştu: {string.Join(", ", errors)}"
            : $"Toplam {cleanedCount} tekrarlanan banner başarıyla silindi."
        ));
      }
      catch (Exception ex)
      {
        return BadRequest(ResultFactory.CreateErrorResult<object>(
          null,
          null,
          "Hata",
          "Tekrarlanan bannerlar temizlenirken hata oluştu.",
          ex.Message
        ));
      }
    }

    /// <summary>
    /// Gelen resim dosyasıyla yeni banner oluşturur (multipart/form-data)
    /// </summary>
    /// <remarks>
    /// Bu uç nokta, gelen resim dosyasıyla yeni banner oluşturur.
    /// </remarks>
    /// <param name="request">Yeni banner bilgilerini içeren istek.</param>
    /// <returns>İşlem sonucunu döndürür.</returns>
    /// <response code="201">Banner başarıyla oluşturuldu.</response>
    /// <response code="400">İstek geçersizse.</response>
    /// <response code="401">Kullanıcı yetkili değilse.</response>
    /// <response code="404">Güncellenecek Banner bulunamazsa.</response>
    [HttpPost("[action]")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Resimli Yeni Banner Oluşturma", Menu = "Banners-Bannerlar")]
    public async Task<ActionResult<TransactionResultPack<CreateBannerWithImageCommandResponse>>> CreateBannerWithImage([FromForm] CreateBannerWithImageCommandRequest request)
    {
      return await SendCommand<CreateBannerWithImageCommandRequest, CreateBannerWithImageCommandResponse>(request);
    }

  }

}
