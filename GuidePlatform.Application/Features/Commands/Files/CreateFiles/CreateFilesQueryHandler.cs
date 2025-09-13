using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Files.CreateFiles
{
  public class CreateFilesCommandHandler : BaseCommandHandler, IRequestHandler<CreateFilesCommandRequest, TransactionResultPack<CreateFilesCommandResponse>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public CreateFilesCommandHandler(
        IApplicationDbContext context,
        IMediator mediator,
        ICurrentUserService currentUserService
    ) : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<CreateFilesCommandResponse>> Handle(CreateFilesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
        // Kullanıcı bilgilerini kontrol et
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 1. files oluştur
        var files = CreateFilesCommandRequest.Map(request, _currentUserService);

        // Auth bilgilerini null yap eğer geçersizse - Set auth info to null if invalid
        // Bu foreign key constraint hatalarını önler - This prevents foreign key constraint errors
        if (files.AuthUserId.HasValue)
        {
          // Burada AuthUser'ın var olup olmadığını kontrol edebilirsiniz
          // You can check if AuthUser exists here
          // Şimdilik null yapıyoruz - For now, we set it to null
          // files.AuthUserId = null;
        }

        await _context.files.AddAsync(files, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(files.Id, request, cancellationToken);

        return ResultFactory.CreateSuccessResult<CreateFilesCommandResponse>(
            new CreateFilesCommandResponse
            {
              StatusCode = (int)HttpStatusCode.Created,
              Id = files.Id
            },
            null,
            null,
            "İşlem Başarılı",
            "files başarıyla oluşturuldu.",
            $"files Id: {files.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateFilesCommandResponse>(
            null,
            null,
            "Hata / İşlem Başarısız",
            "files oluşturulurken bir hata oluştu.",
            ex.InnerException?.Message ?? ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid filesId, CreateFilesCommandRequest request, CancellationToken cancellationToken)
    // {
    //     // Ek işlemler buraya eklenebilir
    //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
    // }
  }
}
