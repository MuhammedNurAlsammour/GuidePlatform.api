using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.UserFavorites.CreateUserFavorites
{
    public class CreateUserFavoritesCommandHandler : BaseCommandHandler, IRequestHandler<CreateUserFavoritesCommandRequest, TransactionResultPack<CreateUserFavoritesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateUserFavoritesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateUserFavoritesCommandResponse>> Handle(CreateUserFavoritesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. userFavorites oluştur
                var userFavorites = CreateUserFavoritesCommandRequest.Map(request, _currentUserService);
                await _context.userFavorites.AddAsync(userFavorites, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(userFavorites.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateUserFavoritesCommandResponse>(
                    new CreateUserFavoritesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = userFavorites.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "userFavorites başarıyla oluşturuldu.",
                    $"userFavorites Id: { userFavorites.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateUserFavoritesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "userFavorites oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid userFavoritesId, CreateUserFavoritesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
