using GuidePlatform.Application.Abstractions.RabbitMQ;
using GuidePlatform.Infrastructure.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace GuidePlatform.Infrastructure
{
	public static class ServiceRegistration
	{
		public static void AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddSingleton<IRabbitMQService, RabbitMQService>();
		}
	}
}
