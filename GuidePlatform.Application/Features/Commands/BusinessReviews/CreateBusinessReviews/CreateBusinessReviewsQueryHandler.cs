using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.BusinessReviews.CreateBusinessReviews
{
    public class CreateBusinessReviewsCommandHandler : BaseCommandHandler, IRequestHandler<CreateBusinessReviewsCommandRequest, TransactionResultPack<CreateBusinessReviewsCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBusinessReviewsCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBusinessReviewsCommandResponse>> Handle(CreateBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. businessReviews oluştur
                var businessReviews = CreateBusinessReviewsCommandRequest.Map(request, _currentUserService);
                await _context.businessReviews.AddAsync(businessReviews, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(businessReviews.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBusinessReviewsCommandResponse>(
                    new CreateBusinessReviewsCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = businessReviews.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "businessReviews başarıyla oluşturuldu.",
                    $"businessReviews Id: { businessReviews.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBusinessReviewsCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "businessReviews oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid businessReviewsId, CreateBusinessReviewsCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
