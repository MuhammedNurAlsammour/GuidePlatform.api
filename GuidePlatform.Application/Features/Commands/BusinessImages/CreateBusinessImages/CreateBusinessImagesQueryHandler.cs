using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;
using GuidePlatform.Application.Services;

namespace GuidePlatform.Application.Features.Commands.BusinessImages.CreateBusinessImages
{
  public class CreateBusinessImagesCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessImagesCommandRequest, TransactionResultPack<CreateBusinessImagesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly IBusinessImageService _businessImageService;

    public CreateBusinessImagesCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IBusinessImageService businessImageService
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
      _businessImageService = businessImageService;
    }

    public async Task<TransactionResultPack<CreateBusinessImagesCommandResponse>> Handle(CreateBusinessImagesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // 1. businessImages oluştur (sadece metadata)
        var businessImages = CreateBusinessImagesCommandRequest.Map(request, _currentUserService);
        await _context.businessImages.AddAsync(businessImages, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Fotoğraf yükleme işlemi (eski sistem için)
        if (!string.IsNullOrEmpty(request.PhotoPath) || !string.IsNullOrEmpty(request.PhotoBase64))
        {
          var photoUploadDto = request.ToPhotoUploadDto(businessImages.BusinessId, businessImages.Id);
          // Fotoğraf güncelleme sonucu kontrol ediliyor
          var photoResult = await _businessImageService.UpdateBusinessImageAsync(photoUploadDto);

          // photoResult.OperationStatus özelliği ile kontrol ediliyor
          if (!photoResult.OperationStatus)
          {
            return ResultFactory.CreateErrorResult<CreateBusinessImagesCommandResponse>(
                businessImages.Id,
                null,
                "Hata / Fotoğraf Yükleme",
                "İşletme görseli oluşturuldu fakat fotoğraf yüklenemedi.",
                "Fotoğraf yüklenemedi."
            );
          }
        }
        // Yeni sistem: PhotoUrl ve ThumbnailUrl zaten Map method'unda kaydedildi

        return ResultFactory.CreateSuccessResult<CreateBusinessImagesCommandResponse>(
            new CreateBusinessImagesCommandResponse
            {
              StatusCode = (int)HttpStatusCode.Created,
              Id = businessImages.Id
            },
            businessImages.Id,
            null,
            "İşlem Başarılı",
            "İşletme görseli başarıyla oluşturuldu.",
            $"İşletme görseli Id: {businessImages.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateBusinessImagesCommandResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "İşletme görseli oluşturulurken bir hata oluştu.",
            ex.InnerException?.Message ?? ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid businessImagesId, CreateBusinessImagesCommandRequest request, CancellationToken cancellationToken)
    // {
    //     // Ek işlemler buraya eklenebilir
    //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
    // }
  }
}
