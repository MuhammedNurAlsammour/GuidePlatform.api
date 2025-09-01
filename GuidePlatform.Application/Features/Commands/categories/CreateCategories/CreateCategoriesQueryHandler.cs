using MediatR;
using Microsoft.EntityFrameworkCore;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.categories.CreateCategories
{
    public class CreateCategoriesCommandHandler : BaseCommandHandler, IRequestHandler<CreateCategoriesCommandRequest, TransactionResultPack<CreateCategoriesCommandResponse>>
  { 
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public CreateCategoriesCommandHandler(IApplicationDbContext context, IMediator mediator, ICurrentUserService currentUserService)
      : base(currentUserService)
    {
      _context = context;
      _mediator = mediator;
    }

    public async Task<TransactionResultPack<CreateCategoriesCommandResponse>> Handle(CreateCategoriesCommandRequest request, CancellationToken cancellationToken)
    {
      try
      {
                // Debug: Kullanıcı bilgilerini kontrol et
        var customerIdFromService = _currentUserService.GetCustomerId();
        var userIdFromService = _currentUserService.GetUserId();

        // 1. categories oluştur
        var categories = CreateCategoriesCommandRequest.Map(request, _currentUserService);
        await _context.categories.AddAsync(categories, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. 🎯 Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
        // await ProcessAdditionalOperationsAsync(categories.Id, request, cancellationToken);

        return ResultFactory.CreateSuccessResult<CreateCategoriesCommandResponse>(
          new CreateCategoriesCommandResponse
          {
            StatusCode = (int)HttpStatusCode.Created,
            Id = categories.Id
          },
          null,
          null,
          "İşlem Başarılı",
          "categories başarıyla oluşturuldu.",
          $"categories Id: { categories.Id} başarıyla oluşturuldu."
        );
      }
      catch (Exception ex)
      {
        return ResultFactory.CreateErrorResult<CreateCategoriesCommandResponse>(
          null,
          null,
          "Hata / İşlem Başarısız",
          "categories oluşturulurken bir hata oluştu.",
          ex.InnerException?.Message ?? ex.Message
        );
      }
    }

    /// <summary>
    /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
    /// </summary>
    // private async Task ProcessAdditionalOperationsAsync(Guid categoriesId, CreateCategoriesCommandRequest request, CancellationToken cancellationToken)
    // {
    //   // Ek işlemler buraya eklenebilir
    //   // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
    // }
  }
}
