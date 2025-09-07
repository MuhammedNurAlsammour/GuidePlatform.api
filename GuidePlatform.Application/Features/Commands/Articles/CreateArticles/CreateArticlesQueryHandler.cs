using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.Features.Commands.Base;

namespace GuidePlatform.Application.Features.Commands.Articles.CreateArticles
{
    public class CreateArticlesCommandHandler : BaseCommandHandler, IRequestHandler<CreateArticlesCommandRequest, TransactionResultPack<CreateArticlesCommandResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateArticlesCommandHandler(
            IApplicationDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService
        ) : base(currentUserService)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<TransactionResultPack<CreateArticlesCommandResponse>> Handle(CreateArticlesCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcı bilgilerini kontrol et
                var customerIdFromService = _currentUserService.GetCustomerId();
                var userIdFromService = _currentUserService.GetUserId();

                // 1. articles oluştur
                var articles = CreateArticlesCommandRequest.Map(request, _currentUserService);
                await _context.articles.AddAsync(articles, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Ek işlemler buraya eklenebilir (örn: ilişkili kayıtlar, validasyonlar, vb.)
                // await ProcessAdditionalOperationsAsync(articles.Id, request, cancellationToken);

                return ResultFactory.CreateSuccessResult<CreateArticlesCommandResponse>(
                    new CreateArticlesCommandResponse
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Id = articles.Id
                    },
                    null,
                    null,
                    "İşlem Başarılı",
                    "articles başarıyla oluşturuldu.",
                    $"articles Id: { articles.Id} başarıyla oluşturuldu."
                );
            }
            catch (Exception ex)
            {
                return ResultFactory.CreateErrorResult<CreateArticlesCommandResponse>(
                    null,
                    null,
                    "Hata / İşlem Başarısız",
                    "articles oluşturulurken bir hata oluştu.",
                    ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        /// <summary>
        /// Ek işlemler için örnek metod - ihtiyaca göre düzenlenebilir
        /// </summary>
        // private async Task ProcessAdditionalOperationsAsync(Guid articlesId, CreateArticlesCommandRequest request, CancellationToken cancellationToken)
        // {
        //     // Ek işlemler buraya eklenebilir
        //     // Örnek: İlişkili kayıtlar oluşturma, validasyonlar, vb.
        // }
    }
}
