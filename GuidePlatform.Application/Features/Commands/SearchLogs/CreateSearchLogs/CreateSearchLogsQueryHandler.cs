using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.SearchLogs.CreateSearchLogs
{
    public class CreateSearchLogsCommandHandler : BaseCommandHandler, IRequestHandler<CreateSearchLogsCommandRequest, TransactionResultPack<CreateSearchLogsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateSearchLogsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateSearchLogsCommandResponse>> Handle(CreateSearchLogsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. searchLogs oluştur
                var searchLogs = CreateSearchLogsCommandRequest.Map(request, _currentUserService);
                await _context.searchLogs.AddAsync(searchLogs, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(searchLogs.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateSearchLogsCommandResponse>(
                    new CreateSearchLogsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = searchLogs.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "searchLogs başarıyla oluşturuldu.",
                    $"searchLogs Id: { searchLogs.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateSearchLogsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "searchLogs oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid searchLogsId, CreateSearchLogsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
