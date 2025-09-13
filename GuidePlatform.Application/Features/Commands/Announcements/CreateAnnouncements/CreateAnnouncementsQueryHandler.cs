using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Announcements.CreateAnnouncements
{
    public class CreateAnnouncementsCommandHandler : BaseCommandHandler, IRequestHandler<CreateAnnouncementsCommandRequest, TransactionResultPack<CreateAnnouncementsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateAnnouncementsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateAnnouncementsCommandResponse>> Handle(CreateAnnouncementsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. announcements oluştur
                var announcements = CreateAnnouncementsCommandRequest.Map(request, _currentUserService);
                await _context.announcements.AddAsync(announcements, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(announcements.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateAnnouncementsCommandResponse>(
                    new CreateAnnouncementsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = announcements.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "announcements başarıyla oluşturuldu.",
                    $"announcements Id: { announcements.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateAnnouncementsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "announcements oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid announcementsId, CreateAnnouncementsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
