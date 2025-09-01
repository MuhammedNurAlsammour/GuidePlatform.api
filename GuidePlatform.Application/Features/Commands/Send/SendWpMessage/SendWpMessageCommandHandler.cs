using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Abstractions.RabbitMQ;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Operations;
using GuidePlatform.Application.RabbitMQ;
using MediatR;
using System.Net;
using System.Text.Json;

namespace GuidePlatform.Application.Features.Commands.Send.SendWpMessage
{
	public class SendWpMessageCommandHandler(IRabbitMQService rabbitMQService) : IRequestHandler<SendWpMessageCommandRequest, TransactionResultPack<SendWpMessageCommandResponse>>
	{
		private readonly IRabbitMQService _rabbitMQService = rabbitMQService;
		public async Task<TransactionResultPack<SendWpMessageCommandResponse>> Handle(SendWpMessageCommandRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var _jsonOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var result = await _rabbitMQService.SendMessage(QueueNames.WpMessage, JsonSerializer.Serialize(request, _jsonOptions));



				return ResultFactory.CreateSuccessResult<SendWpMessageCommandResponse>(
					new(),
					null,
					null,
					"İşlem Başarılı",
					"SendWpMessage başarıyla oluşturuldu.",
					$""
				);
			}
			catch (Exception ex)
			{
				return ResultFactory.CreateErrorResult<SendWpMessageCommandResponse>(
					null,
					null,
					"Hata / İşlem Başarısız",
					"SendWpMessage oluşturulurken bir hata oluştu.",
					ex.Message
				);
			}
		}
	}


}
