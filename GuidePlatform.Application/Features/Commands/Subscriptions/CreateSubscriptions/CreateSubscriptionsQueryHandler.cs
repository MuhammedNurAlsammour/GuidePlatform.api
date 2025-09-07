using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Subscriptions.CreateSubscriptions
{
    public class CreateSubscriptionsCommandHandler : BaseCommandHandler, IRequestHandler<CreateSubscriptionsCommandRequest, TransactionResultPack<CreateSubscriptionsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateSubscriptionsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateSubscriptionsCommandResponse>> Handle(CreateSubscriptionsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. subscriptions oluştur
                var subscriptions = CreateSubscriptionsCommandRequest.Map(request, _currentUserService);
                await _context.subscriptions.AddAsync(subscriptions, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(subscriptions.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateSubscriptionsCommandResponse>(
                    new CreateSubscriptionsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = subscriptions.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "subscriptions başarıyla oluşturuldu.",
                    $"subscriptions Id: { subscriptions.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateSubscriptionsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "subscriptions oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid subscriptionsId, CreateSubscriptionsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
