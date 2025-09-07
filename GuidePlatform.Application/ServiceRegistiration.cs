using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Application.Services;
using Karmed.External.Auth.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

      // BusinessImageService'i kaydet
      services.AddScoped<IBusinessImageService, BusinessImageService>();

      // 🎯 OData Handler'ları için gerekli servisleri kaydet
      // Bu servisler Program.cs'de zaten kayıtlı ama emin olmak için buraya da ekliyoruz
      services.AddScoped<ICurrentUserService, CurrentUserService>();

      // 🎯 Diğer application services buraya eklenebilir
    }
  }
}
