using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Payments.CreatePayments
{
    public class CreatePaymentsCommandHandler : BaseCommandHandler, IRequestHandler<CreatePaymentsCommandRequest, TransactionResultPack<CreatePaymentsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreatePaymentsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreatePaymentsCommandResponse>> Handle(CreatePaymentsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. payments oluştur
                var payments = CreatePaymentsCommandRequest.Map(request, _currentUserService);
                await _context.payments.AddAsync(payments, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(payments.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreatePaymentsCommandResponse>(
                    new CreatePaymentsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = payments.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "payments başarıyla oluşturuldu.",
                    $"payments Id: { payments.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreatePaymentsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "payments oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid paymentsId, CreatePaymentsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
