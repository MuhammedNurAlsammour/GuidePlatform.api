using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Features.Commands.Banners.CreateBanners;
using GuidePlatform.Application.Features.Commands.Banners.UpdateBanners;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBannerWithImage
{
  /// <summary>
  /// Flutter'dan multipart/form-data ile gelen banner oluşturma işlemini yöneten handler
  /// </summary>
  public class CreateBannerWithImageCommandHandler : BaseCommandHandler, IRequestHandler<CreateBannerWithImageCommandRequest, TransactionResultPack<CreateBannerWithImageCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CreateBannerWithImageCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
      _httpContextAccessor = httpContextAccessor;
      _configuration = configuration;
    }

    public async Task<TransactionResultPack<CreateBannerWithImageCommandResponse>> Handle(CreateBannerWithImageCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 1. أولاً إنشاء البانر بدون صورة
        var createRequest = new CreateBannerWithImageCommandRequest
        {
          AuthCustomerId = request.AuthCustomerId,
          AuthUserId = request.AuthUserId,
          CreateUserId = request.CreateUserId,
          UpdateUserId = request.UpdateUserId,
          Title = request.Title,
          ProvinceId = request.ProvinceId,
          Description = request.Description,
          PhotoContentType = request.PhotoFile?.ContentType ?? "image/jpeg",
          LinkUrl = request.LinkUrl,
          StartDate = request.StartDate,
          EndDate = request.EndDate,
          IsActive = request.IsActive,
          OrderIndex = request.OrderIndex,
          Icon = request.Icon
        };

        // إنشاء البانر أولاً
        var bannerResult = await _mediator.Send(createRequest, cancellationToken);

        if (!bannerResult.OperationStatus || bannerResult.Result == null)
        {
          return ResultFactory.CreateErrorResult<CreateBannerWithImageCommandResponse>(
            null,
            null,
            "Hata / Banner Oluşturma",
            "Banner oluşturulurken hata oluştu.",
            bannerResult.OperationResult?.MessageDetail ?? "Banner oluşturulamadı."
          );
        }

        // 2. إذا كان هناك صورة، احفظها مباشرة في wwwroot
        string? imageUrl = null;
        if (request.PhotoFile != null && request.PhotoFile.Length > 0)
        {
          var bannerId = bannerResult.Result.Id;
          var result = await SaveImageDirectlyToWwwroot(request.PhotoFile, bannerId);

          if (!result.Success)
          {
            return ResultFactory.CreateErrorResult<CreateBannerWithImageCommandResponse>(
              bannerId.ToString(),
              null,
              "Hata / Fotoğraf Kaydetme",
              "Fotoğraf wwwroot'a kaydedilemedi.",
              result.ErrorMessage
            );
          }

          imageUrl = result.ImageUrl;
        }

        return ResultFactory.CreateSuccessResult<CreateBannerWithImageCommandResponse>(
          new CreateBannerWithImageCommandResponse
          {
            Id = bannerResult.Result.Id,
            StatusCode = (int)HttpStatusCode.Created,
            Message = "Banner başarıyla oluşturuldu",
            ImageUrl = imageUrl
          },
          bannerResult.Result.Id.ToString(),
          null,
          "İşlem Başarılı",
          "Banner başarıyla oluşturuldu.",
          $"Banner ID: {bannerResult.Result.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateBannerWithImageCommandResponse>(
          null,
          null,
          "Hata / Form Data",
          "Form verisi işlenirken hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// حفظ الصورة مباشرة في wwwroot بدون Base64
    /// </summary>
    private async Task<(bool Success, string? ErrorMessage, string? ImageUrl)> SaveImageDirectlyToWwwroot(IFormFile imageFile, Guid bannerId)
    {
      try
      {
        // إنشاء اسم ملف فريد
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomId = Guid.NewGuid().ToString("N")[..8];
        var extension = Path.GetExtension(imageFile.FileName) ?? ".jpg";
        var fileName = $"banner_{bannerId}_photo_{timestamp}_{randomId}{extension}";

        // مسار المجلد
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(webRootPath, "images", "banners");

        // إنشاء المجلد إذا لم يكن موجود
        if (!Directory.Exists(imagesPath))
        {
          Directory.CreateDirectory(imagesPath);
        }

        // مسار الملف الكامل
        var filePath = Path.Combine(imagesPath, fileName);

        // حفظ الملف
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await imageFile.CopyToAsync(stream);
        }

        // إنشاء URL للصورة من appsettings.json
        var baseUrl = _configuration["BaseUrl"] ?? "http://72.60.33.111:3000/guideapi";
        var imageUrl = $"{baseUrl.TrimEnd('/')}/images/banners/{fileName}";

        // تحديث البانر في قاعدة البيانات بـ URL الصورة
        await UpdateBannerImageUrl(bannerId, imageUrl);

        return (true, null, imageUrl);
      }
      catch (Exception ex)
      {
        return (false, ex.Message, null);
      }
    }

    /// <summary>
    /// تحديث URL الصورة في قاعدة البيانات
    /// </summary>
    private async Task UpdateBannerImageUrl(Guid bannerId, string imageUrl)
    {
      try
      {
        var updateRequest = new UpdateBannersCommandRequest
        {
          Id = bannerId.ToString(),
          PhotoUrl = imageUrl,
          ThumbnailUrl = imageUrl // نفس الصورة للـ thumbnail مؤقتاً
        };

        await _mediator.Send(updateRequest);
      }
      catch
      {
        // تجاهل أخطاء التحديث مؤقتاً
      }
    }
  }
}
