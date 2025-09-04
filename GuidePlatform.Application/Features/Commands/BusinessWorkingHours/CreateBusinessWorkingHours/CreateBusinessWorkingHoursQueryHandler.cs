using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessWorkingHours.CreateBusinessWorkingHours
{
    public class CreateBusinessWorkingHoursCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessWorkingHoursCommandRequest, TransactionResultPack<CreateBusinessWorkingHoursCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessWorkingHoursCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessWorkingHoursCommandResponse>> Handle(CreateBusinessWorkingHoursCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. businessWorkingHours oluştur
                var businessWorkingHours = CreateBusinessWorkingHoursCommandRequest.Map(request, _currentUserService);
                await _context.businessWorkingHours.AddAsync(businessWorkingHours, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businessWorkingHours.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessWorkingHoursCommandResponse>(
                    new CreateBusinessWorkingHoursCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businessWorkingHours.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "businessWorkingHours başarıyla oluşturuldu.",
                    $"businessWorkingHours Id: { businessWorkingHours.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessWorkingHoursCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessWorkingHours oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessWorkingHoursId, CreateBusinessWorkingHoursCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
