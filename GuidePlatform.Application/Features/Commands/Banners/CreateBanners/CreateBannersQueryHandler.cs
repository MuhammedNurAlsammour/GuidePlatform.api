using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Banners.CreateBanners
{
    public class CreateBannersCommandHandler : BaseCommandHandler, IRequestHandler<CreateBannersCommandRequest, TransactionResultPack<CreateBannersCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateBannersCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateBannersCommandResponse>> Handle(CreateBannersCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. banners oluştur
                var banners = CreateBannersCommandRequest.Map(request, _currentUserService);
                await _context.banners.AddAsync(banners, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(banners.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateBannersCommandResponse>(
                    new CreateBannersCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = banners.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "banners başarıyla oluşturuldu.",
                    $"banners Id: { banners.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateBannersCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "banners oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid bannersId, CreateBannersCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
