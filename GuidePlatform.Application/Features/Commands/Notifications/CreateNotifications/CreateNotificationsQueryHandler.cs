using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Notifications.CreateNotifications
{
    public class CreateNotificationsCommandHandler : BaseCommandHandler, IRequestHandler<CreateNotificationsCommandRequest, TransactionResultPack<CreateNotificationsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateNotificationsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateNotificationsCommandResponse>> Handle(CreateNotificationsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. notifications oluştur
                var notifications = CreateNotificationsCommandRequest.Map(request, _currentUserService);
                await _context.notifications.AddAsync(notifications, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(notifications.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateNotificationsCommandResponse>(
                    new CreateNotificationsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = notifications.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "notifications başarıyla oluşturuldu.",
                    $"notifications Id: { notifications.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateNotificationsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "notifications oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid notificationsId, CreateNotificationsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
