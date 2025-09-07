using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Pages.CreatePages
{
    public class CreatePagesCommandHandler : BaseCommandHandler, IRequestHandler<CreatePagesCommandRequest, TransactionResultPack<CreatePagesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreatePagesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreatePagesCommandResponse>> Handle(CreatePagesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. pages oluştur
                var pages = CreatePagesCommandRequest.Map(request, _currentUserService);
                await _context.pages.AddAsync(pages, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(pages.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreatePagesCommandResponse>(
                    new CreatePagesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = pages.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "pages başarıyla oluşturuldu.",
                    $"pages Id: { pages.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreatePagesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "pages oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid pagesId, CreatePagesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
