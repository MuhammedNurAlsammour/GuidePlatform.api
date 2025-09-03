using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Businesses.CreateBusinesses
{
    public class CreateBusinessesCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessesCommandRequest, TransactionResultPack<CreateBusinessesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessesCommandResponse>> Handle(CreateBusinessesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. Businesses oluştur
                var businesses = CreateBusinessesCommandRequest.Map(request, _currentUserService);
                await _context.businesses.AddAsync(businesses, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businesses.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessesCommandResponse>(
                    new CreateBusinessesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businesses.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "Businesses başarıyla oluşturuldu.",
                    $"Businesses Id: { businesses.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "Businesses oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessesId, CreateBusinessesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
