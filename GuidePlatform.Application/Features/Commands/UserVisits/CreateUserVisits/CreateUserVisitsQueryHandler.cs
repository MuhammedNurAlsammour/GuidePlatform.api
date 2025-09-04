using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.UserVisits.CreateUserVisits
{
    public class CreateUserVisitsCommandHandler : BaseCommandHandler, IRequestHandler<CreateUserVisitsCommandRequest, TransactionResultPack<CreateUserVisitsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateUserVisitsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateUserVisitsCommandResponse>> Handle(CreateUserVisitsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. userVisits oluştur
                var userVisits = CreateUserVisitsCommandRequest.Map(request, _currentUserService);
                await _context.userVisits.AddAsync(userVisits, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(userVisits.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateUserVisitsCommandResponse>(
                    new CreateUserVisitsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = userVisits.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "userVisits başarıyla oluşturuldu.",
                    $"userVisits Id: { userVisits.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateUserVisitsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "userVisits oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid userVisitsId, CreateUserVisitsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
