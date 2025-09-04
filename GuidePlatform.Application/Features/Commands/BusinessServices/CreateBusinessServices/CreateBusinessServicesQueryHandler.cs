using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessServices.CreateBusinessServices
{
    public class CreateBusinessServicesCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessServicesCommandRequest, TransactionResultPack<CreateBusinessServicesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessServicesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessServicesCommandResponse>> Handle(CreateBusinessServicesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. businessServices oluştur
                var businessServices = CreateBusinessServicesCommandRequest.Map(request, _currentUserService);
                await _context.businessServices.AddAsync(businessServices, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businessServices.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessServicesCommandResponse>(
                    new CreateBusinessServicesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businessServices.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "businessServices başarıyla oluşturuldu.",
                    $"businessServices Id: { businessServices.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessServicesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessServices oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessServicesId, CreateBusinessServicesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
