using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessAnalytics.CreateBusinessAnalytics
{
    public class CreateBusinessAnalyticsCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessAnalyticsCommandRequest, TransactionResultPack<CreateBusinessAnalyticsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessAnalyticsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessAnalyticsCommandResponse>> Handle(CreateBusinessAnalyticsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. businessAnalytics oluştur
                var businessAnalytics = CreateBusinessAnalyticsCommandRequest.Map(request, _currentUserService);
                await _context.businessAnalytics.AddAsync(businessAnalytics, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businessAnalytics.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessAnalyticsCommandResponse>(
                    new CreateBusinessAnalyticsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businessAnalytics.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "businessAnalytics başarıyla oluşturuldu.",
                    $"businessAnalytics Id: { businessAnalytics.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessAnalyticsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessAnalytics oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessAnalyticsId, CreateBusinessAnalyticsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
