using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Persistence.Contexts;
using GuidePlatform.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuidePlatform.Persistence
{
	public static class ServiceRegistration
	{
		public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<CreateAndUpdateDateInterceptor>();

			services.AddDbContext<ApplicationDbContext>((sp, options) =>
			{
				var createAndUpdateDateInterceptor = sp.GetRequiredService<CreateAndUpdateDateInterceptor>();

				// 🎯 PostgreSQL bağlantı ayarları - PostgreSQL bağlantı ayarları
				var connectionString = configuration.GetConnectionString("PostgreSQL");
				
				options.UseNpgsql(connectionString, npgsqlOptions =>
				{
					// 🎯 Timeout ayarları - Timeout ayarları
					npgsqlOptions.CommandTimeout(60); // 60 saniye komut timeout'u (artırıldı)
					npgsqlOptions.EnableRetryOnFailure(
						maxRetryCount: 5, // Maksimum 5 deneme (artırıldı)
						maxRetryDelay: TimeSpan.FromSeconds(30), // Maksimum 30 saniye bekleme (artırıldı)
						errorCodesToAdd: null // Tüm hatalar için retry
					);
					
				})
				.UseSnakeCaseNamingConvention()
				.AddInterceptors(createAndUpdateDateInterceptor)
				.EnableSensitiveDataLogging(false) // Production'da false olmalı
				.EnableDetailedErrors(true); // Development için true

			}, ServiceLifetime.Scoped);

			services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

			// 🎯 SharedDbContext'i kaydet (storeplatformh schema için)
			services.AddDbContext<SharedDbContext>((sp, options) =>
			{
				var createAndUpdateDateInterceptor = sp.GetRequiredService<CreateAndUpdateDateInterceptor>();

				// 🎯 PostgreSQL bağlantı ayarları - Shared schema için aynı bağlantı
				var connectionString = configuration.GetConnectionString("PostgreSQL");
				
				options.UseNpgsql(connectionString, npgsqlOptions =>
				{
					// 🎯 Timeout ayarları - Timeout ayarları
					npgsqlOptions.CommandTimeout(60); // 60 saniye komut timeout'u
					npgsqlOptions.EnableRetryOnFailure(
						maxRetryCount: 5, // Maksimum 5 deneme
						maxRetryDelay: TimeSpan.FromSeconds(30), // Maksimum 30 saniye bekleme
						errorCodesToAdd: null // Tüm hatalar için retry
					);
					
				})
				.UseSnakeCaseNamingConvention()
				.AddInterceptors(createAndUpdateDateInterceptor)
				.EnableSensitiveDataLogging(false) // Production'da false olmalı
				.EnableDetailedErrors(true); // Development için true

			}, ServiceLifetime.Scoped);

			services.AddScoped<ISharedDbContext, SharedDbContext>();
		}
	}
}