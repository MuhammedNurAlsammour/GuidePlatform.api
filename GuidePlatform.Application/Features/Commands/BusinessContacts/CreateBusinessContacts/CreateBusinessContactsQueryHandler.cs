using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessContacts.CreateBusinessContacts
{
    public class CreateBusinessContactsCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessContactsCommandRequest, TransactionResultPack<CreateBusinessContactsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessContactsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessContactsCommandResponse>> Handle(CreateBusinessContactsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. businessContacts oluştur
                var businessContacts = CreateBusinessContactsCommandRequest.Map(request, _currentUserService);
                await _context.businessContacts.AddAsync(businessContacts, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businessContacts.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessContactsCommandResponse>(
                    new CreateBusinessContactsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businessContacts.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "businessContacts başarıyla oluşturuldu.",
                    $"businessContacts Id: { businessContacts.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessContactsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessContacts oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessContactsId, CreateBusinessContactsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
