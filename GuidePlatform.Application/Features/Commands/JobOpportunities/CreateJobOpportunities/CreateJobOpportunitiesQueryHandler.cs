using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.JobOpportunities.CreateJobOpportunities
{
    public class CreateJobOpportunitiesCommandHandler : BaseCommandHandler, IRequestHandler<CreateJobOpportunitiesCommandRequest, TransactionResultPack<CreateJobOpportunitiesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateJobOpportunitiesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateJobOpportunitiesCommandResponse>> Handle(CreateJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. jobOpportunities oluştur
                var jobOpportunities = CreateJobOpportunitiesCommandRequest.Map(request, _currentUserService);
                await _context.jobOpportunities.AddAsync(jobOpportunities, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(jobOpportunities.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateJobOpportunitiesCommandResponse>(
                    new CreateJobOpportunitiesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = jobOpportunities.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "jobOpportunities başarıyla oluşturuldu.",
                    $"jobOpportunities Id: { jobOpportunities.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateJobOpportunitiesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "jobOpportunities oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid jobOpportunitiesId, CreateJobOpportunitiesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
