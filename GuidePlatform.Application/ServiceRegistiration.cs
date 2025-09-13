using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Services;
using Karmed.External.Auth.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace GuidePlatform.Application
{
  public static class ServiceRegistiration
  {
    public static void AddApplicationServices(this IServiceCollection services)
    {
      services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ServiceRegistiration).GetTypeInfo().Assembly));

      // 🎯 AuthUserDetailService'i kaydet
      services.AddScoped<IAuthUserDetailService, AuthUserDetailService>();

      //  ImageService'i kaydet (temel resim işleme servisi)
      services.AddScoped<IImageService, ImageService>();

      // FileStorageService'i kaydet (wwwroot dosya yönetimi)
      services.AddScoped<IFileStorageService>(provider =>
      {
        var webHostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["BaseUrl"] ?? "https://localhost:2029";
        return new FileStorageService(webHostEnvironment.WebRootPath, baseUrl);
      });

      // BusinessImageService'i kaydet
      services.AddScoped<IBusinessImageService, BusinessImageService>();

      // BannerImageService'i kaydet
      services.AddScoped<IBannerImageService, BannerImageService>();

      // 🎯 OData Handler'ları için gerekli servisleri kaydet
      // Bu servisler Program.cs'de zaten kayıtlı ama emin olmak için buraya da ekliyoruz
      services.AddScoped<ICurrentUserService, CurrentUserService>();

      // 🎯 SharedApiService'i kaydet (HttpClient yerine DbContext kullanıyor)
      services.AddScoped<ISharedApiService, SharedApiService>();

      // 🎯 Diğer application services buraya eklenebilir
    }
  }
}
