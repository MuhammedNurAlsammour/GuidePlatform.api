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
using GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImages;
using GuidePlatform.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImagesWithImage
{
  /// <summary>
  /// Flutter'dan multipart/form-data ile gelen BusinessImages oluşturma işlemini yöneten handler
  /// </summary>
  public class CreateBusinessImagesWithImageCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessImagesWithImageCommandRequest, TransactionResultPack<CreateBusinessImagesWithImageCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IImageService _imageService;

    public CreateBusinessImagesWithImageCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IImageService imageService
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
      _httpContextAccessor = httpContextAccessor;
      _configuration = configuration;
      _imageService = imageService;
    }

    public async Task<TransactionResultPack<CreateBusinessImagesWithImageCommandResponse>> Handle(CreateBusinessImagesWithImageCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 1. التحقق من وجود Business ID في قاعدة البيانات
        var businessExists = await _context.businesses
          .AnyAsync(b => b.Id == request.BusinessId && b.RowIsActive && !b.RowIsDeleted, cancellationToken);

        if (!businessExists)
        {
          return ResultFactory.CreateErrorResult<CreateBusinessImagesWithImageCommandResponse>(
            null,
            null,
            "Hata / Business Bulunamadı",
            "Belirtilen Business ID bulunamadı.",
            $"Business ID: {request.BusinessId} mevcut değil veya silinmiş."
          );
        }

        // 2. أولاً إنشاء BusinessImages بدون صورة
        var createRequest = new CreateBusinessImagesCommandRequest
        {
          AuthCustomerId = request.AuthCustomerId,
          AuthUserId = request.AuthUserId,
          CreateUserId = request.CreateUserId,
          UpdateUserId = request.UpdateUserId,
          BusinessId = request.BusinessId,
          AltText = request.AltText,
          IsPrimary = request.IsPrimary,
          SortOrder = request.SortOrder,
          Icon = request.Icon ?? "image",
          ImageType = request.ImageType
        };

        // إنشاء BusinessImages أولاً
        var businessImageResult = await _mediator.Send(createRequest, cancellationToken);

        if (!businessImageResult.OperationStatus || businessImageResult.Result == null)
        {
          return ResultFactory.CreateErrorResult<CreateBusinessImagesWithImageCommandResponse>(
            null,
            null,
            "Hata / BusinessImages Oluşturma",
            "BusinessImages oluşturulurken hata oluştu.",
            businessImageResult.OperationResult?.MessageDetail ?? "BusinessImages oluşturulamadı."
          );
        }

        var businessImageId = businessImageResult.Result.Id;

        // 3. إذا كان هناك صورة، احفظها مباشرة في wwwroot مع thumbnail
        string? imageUrl = null;
        string? thumbnailUrl = null;
        if (request.PhotoFile != null && request.PhotoFile.Length > 0)
        {
          var result = await SaveImageDirectlyToWwwroot(request.PhotoFile, businessImageId, request.BusinessId);

          if (!result.Success)
          {
            return ResultFactory.CreateErrorResult<CreateBusinessImagesWithImageCommandResponse>(
              businessImageId.ToString(),
              null,
              "Hata / Fotoğraf Kaydetme",
              "Fotoğraf wwwroot'a kaydedilemedi.",
              result.ErrorMessage
            );
          }

          imageUrl = result.ImageUrl;
          thumbnailUrl = result.ThumbnailUrl;

          // 4. تحديث BusinessImages في قاعدة البيانات بـ URLs الصور
          var updateSuccess = await UpdateBusinessImageUrl(businessImageId, imageUrl, thumbnailUrl);

          if (!updateSuccess)
          {
            // تحذير: الصور تم حفظها لكن URLs لم يتم تحديثها في قاعدة البيانات
            System.Diagnostics.Debug.WriteLine($"Warning: Images saved but URLs not updated in database for BusinessImage ID: {businessImageId}");

            // يمكن إضافة log أو إرجاع تحذير للمستخدم
            // لكن العملية ستكمل بنجاح لأن الصور تم حفظها
          }
        }

        return ResultFactory.CreateSuccessResult<CreateBusinessImagesWithImageCommandResponse>(
          new CreateBusinessImagesWithImageCommandResponse
          {
            Id = businessImageResult.Result.Id,
            StatusCode = (int)HttpStatusCode.Created,
            Message = "BusinessImages başarıyla oluşturuldu",
            ImageUrl = imageUrl,
            ThumbnailUrl = thumbnailUrl
          },
          businessImageResult.Result.Id.ToString(),
          null,
          "İşlem Başarılı",
          "BusinessImages başarıyla oluşturuldu.",
          $"BusinessImages ID: {businessImageResult.Result.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateBusinessImagesWithImageCommandResponse>(
          null,
          null,
          "Hata / Form Data",
          "Form verisi işlenirken hata oluştu.",
          ex.Message
        );
      }
    }

    /// <summary>
    /// حفظ الصورة مباشرة في wwwroot مع إنشاء thumbnail أصغر
    /// </summary>
    private async Task<(bool Success, string? ErrorMessage, string? ImageUrl, string? ThumbnailUrl)> SaveImageDirectlyToWwwroot(IFormFile imageFile, Guid businessImageId, Guid businessId)
    {
      try
      {
        // إنشاء اسم ملف فريد
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomId = Guid.NewGuid().ToString("N")[..8];
        var extension = Path.GetExtension(imageFile.FileName) ?? ".jpg";
        var fileName = $"business_{businessId}_image_{businessImageId}_photo_{timestamp}_{randomId}{extension}";
        var thumbnailFileName = $"business_{businessId}_image_{businessImageId}_thumb_{timestamp}_{randomId}{extension}";

        // مسار المجلد
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(webRootPath, "images", "businesses", businessId.ToString());

        // إنشاء المجلد إذا لم يكن موجود
        if (!Directory.Exists(imagesPath))
        {
          Directory.CreateDirectory(imagesPath);
        }

        // مسار الملف الكامل
        var filePath = Path.Combine(imagesPath, fileName);
        var thumbnailFilePath = Path.Combine(imagesPath, thumbnailFileName);

        // حفظ الصورة الأصلية
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await imageFile.CopyToAsync(stream);
        }

        // إنشاء thumbnail أصغر باستخدام ImageService
        var imageData = await File.ReadAllBytesAsync(filePath);
        var thumbnailData = await GenerateThumbnailAsync(imageData);

        // حفظ thumbnail
        await File.WriteAllBytesAsync(thumbnailFilePath, thumbnailData);

        // إنشاء URLs للصور من appsettings.json
        var baseUrl = _configuration["BaseUrl"] ?? "http://72.60.33.111:3000/guideapi";
        var imageUrl = $"{baseUrl.TrimEnd('/')}/images/businesses/{businessId}/{fileName}";
        var thumbnailUrl = $"{baseUrl.TrimEnd('/')}/images/businesses/{businessId}/{thumbnailFileName}";

        return (true, null, imageUrl, thumbnailUrl);
      }
      catch (Exception ex)
      {
        return (false, ex.Message, null, null);
      }
    }

    /// <summary>
    /// إنشاء thumbnail أصغر من الصورة الأصلية
    /// </summary>
    private async Task<byte[]> GenerateThumbnailAsync(byte[] originalImage)
    {
      try
      {
        using (var image = Image.Load(originalImage))
        {
          // تحديد حجم thumbnail (30x30 بكسل)
          const int thumbnailSize = 30;

          image.Mutate(x => x
            .Resize(new ResizeOptions
            {
              Size = new Size(thumbnailSize, thumbnailSize),
              Mode = ResizeMode.Max
            }));

          using (var ms = new MemoryStream())
          {
            await image.SaveAsJpegAsync(ms);
            return ms.ToArray();
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Thumbnail oluşturulurken hata oluştu: {ex.Message}", ex);
      }
    }

    /// <summary>
    /// تحديث URL الصورة في قاعدة البيانات مباشرة
    /// </summary>
    private async Task<bool> UpdateBusinessImageUrl(Guid businessImageId, string imageUrl, string thumbnailUrl)
    {
      try
      {
        // البحث عن السجل وتحديثه مباشرة
        var businessImage = await _context.businessImages
          .FirstOrDefaultAsync(bi => bi.Id == businessImageId);

        if (businessImage == null)
        {
          System.Diagnostics.Debug.WriteLine($"BusinessImage with ID {businessImageId} not found");
          return false;
        }

        // تحديث URLs فقط
        businessImage.PhotoUrl = imageUrl;
        businessImage.ThumbnailUrl = thumbnailUrl;
        businessImage.RowUpdatedDate = DateTime.UtcNow;

        // حفظ التغييرات
        var changesCount = await _context.SaveChangesAsync();

        System.Diagnostics.Debug.WriteLine($"Updated BusinessImage {businessImageId}: PhotoUrl={imageUrl}, ThumbnailUrl={thumbnailUrl}, Changes={changesCount}");

        return changesCount > 0;
      }
      catch (Exception ex)
      {
        // إضافة تسجيل الخطأ
        System.Diagnostics.Debug.WriteLine($"Error updating BusinessImage URLs: {ex.Message}");
        return false;
      }
    }
  }
}
