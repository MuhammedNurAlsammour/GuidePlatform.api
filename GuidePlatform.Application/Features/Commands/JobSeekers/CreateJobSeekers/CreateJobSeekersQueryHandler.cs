using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.JobSeekers.CreateJobSeekers
{
    public class CreateJobSeekersCommandHandler : BaseCommandHandler, IRequestHandler<CreateJobSeekersCommandRequest, TransactionResultPack<CreateJobSeekersCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateJobSeekersCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateJobSeekersCommandResponse>> Handle(CreateJobSeekersCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. jobSeekers oluştur
                var jobSeekers = CreateJobSeekersCommandRequest.Map(request, _currentUserService);
                await _context.jobSeekers.AddAsync(jobSeekers, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(jobSeekers.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateJobSeekersCommandResponse>(
                    new CreateJobSeekersCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = jobSeekers.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "jobSeekers başarıyla oluşturuldu.",
                    $"jobSeekers Id: { jobSeekers.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateJobSeekersCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "jobSeekers oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid jobSeekersId, CreateJobSeekersCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
