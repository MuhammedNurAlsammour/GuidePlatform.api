using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.NotificationSettings.CreateNotificationSettings
{
    public class CreateNotificationSettingsCommandHandler : BaseCommandHandler, IRequestHandler<CreateNotificationSettingsCommandRequest, TransactionResultPack<CreateNotificationSettingsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateNotificationSettingsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateNotificationSettingsCommandResponse>> Handle(CreateNotificationSettingsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. notificationSettings oluştur
                var notificationSettings = CreateNotificationSettingsCommandRequest.Map(request, _currentUserService);
                await _context.notificationSettings.AddAsync(notificationSettings, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(notificationSettings.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateNotificationSettingsCommandResponse>(
                    new CreateNotificationSettingsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = notificationSettings.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "notificationSettings başarıyla oluşturuldu.",
                    $"notificationSettings Id: { notificationSettings.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateNotificationSettingsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "notificationSettings oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid notificationSettingsId, CreateNotificationSettingsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
