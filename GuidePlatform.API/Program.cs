using FluentValidation.AspNetCore;
using GuidePlatform.API.Filters;
using GuidePlatform.Application;
using GuidePlatform.Application;
using GuidePlatform.Infrastructure;
using GuidePlatform.Infrastructure;
using GuidePlatform.Infrastructure.Filters;
using GuidePlatform.Infrastructure.Filters;
using GuidePlatform.Persistence;
using GuidePlatform.Persistence;
using GuidePlatform.Persistence.Contexts;
using GuidePlatform.Persistence.Interceptors;
using Karmed.External.Auth.Library.Contexts;
using Karmed.External.Auth.Library.Entities.Identity;
using Karmed.External.Auth.Library.Filters;
using Karmed.External.Auth.Library.Repositories;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.ClearProviders(); // Clear default providers if you want to start fresh
builder.Logging.AddConsole(); // Console logging
builder.Logging.AddDebug();   // Debug window logging

// Configure detailed logging levels
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Logging.AddFilter("GuidePlatform", LogLevel.Debug);

builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(
	options =>
	{
		var supportedCultures = new List<CultureInfo>
		{
			new CultureInfo("en-US"),
			new CultureInfo("tr-TR"),
		};

		options.DefaultRequestCulture = new RequestCulture(culture: "tr-TR", uiCulture: "tr-TR");
		options.SupportedCultures = supportedCultures;
		options.SupportedUICultures = supportedCultures;
	});

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddHttpClient();

// Add required services from external auth library
builder.Services.AddScoped<ICurrentUserService, Karmed.External.Auth.Library.Services.CurrentUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IMenuReadRepository, MenuReadRepository>();
builder.Services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();
builder.Services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
builder.Services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();

// Add database context for auth services
builder.Services.AddDbContext<AuthDbContext>((sp, options) =>
{
	options.UseNpgsql(builder.Configuration["Auth:ConnectionString"]);
});

// Add Identity services
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<AuthDbContext>();

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);
builder.Services.AddCors(
  options => options.AddDefaultPolicy(policy =>
	policy.WithOrigins(
			// Docker Network IPs - Ana ağ adresleri
			"http://10.10.0.2:3000",
			"https://10.10.0.2:3000",
			"http://10.10.0.2:4200",
			"https://10.10.0.2:4200",
			"http://10.10.0.2:2050",
			"https://10.10.0.2:2050",
			"http://10.10.0.2:2051",
			"https://10.10.0.2:2051",

			// Localhost - Yerel geliştirme
			"http://localhost:3000",
			"https://localhost:3000",
			"http://localhost:4200",
			"https://localhost:4200",
			"http://localhost:2050",
			"https://localhost:2050",
			"http://localhost:2051",
			"https://localhost:2051",
			"http://localhost:2029",
			"https://localhost:2029",
			"http://localhost",
			"https://localhost",

			// Production IPs - Üretim IP'leri
			"http://72.60.33.111:3000",
			"https://72.60.33.111:3000",
			"http://72.60.33.111:4200",
			"https://72.60.33.111:4200",
			"http://72.60.33.111:2050",
			"https://72.60.33.111:2050",
			"http://72.60.33.111:2051",
			"https://72.60.33.111:2051",
			"http://72.60.33.111",
			"https://72.60.33.111",

			// External Domains - Dış domain'ler
			"https://hrefpro.kardelenyazilim.com",
			"http://hrefpro.kardelenyazilim.com",
			"https://193.3.35.117:443",
			"http://193.3.35.117:80",
			"https://193.3.35.117",
			"http://193.3.35.117",

			// Development IPs - Geliştirme IP'leri
			"http://192.168.1.232:2024",
			"https://192.168.1.232:2024",
			"http://192.168.1.232",
			"https://192.168.1.232"
		)
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials() // Credentials'ı etkinleştir - Kimlik bilgilerini etkinleştir
  )
);

builder.Services.AddAuthorization();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services
	.AddControllers(options =>
	{
		options.Filters.Add<ValidationFilter>();
	})
	.AddDataAnnotationsLocalization()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.WriteIndented = true;
	})
	.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
	;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.CustomSchemaIds(type => type.FullName);
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer("Admin", options =>
{
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new()
	{
		ValidateAudience = true,
		ValidateIssuer = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidAudience = builder.Configuration["Token:Audience"],
		ValidIssuer = builder.Configuration["Token:Issuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
		LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null && expires > DateTime.UtcNow,
		NameClaimType = ClaimTypes.Name,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Guide Platform Api",
		Version = "v1.0.2",
		Description = "GuidePlatform",
		Contact = new()
		{
			Name = "Muhammed Nur Alsamour",
			Email = "muhammed2005nour@gmail.com"
		}
	});
	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});

	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
	{
		new OpenApiSecurityScheme
		{
			Reference = new OpenApiReference
			{
				Type=ReferenceType.SecurityScheme,
				Id="Bearer"
			}
		},
		new string[]{}
	}
	});

	var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
	opt.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

var app = builder.Build();
var localizeOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizeOptions.Value);

app.UseSwagger();
app.UseSwaggerUI();

// CORS middleware'ini en başa taşı - CORS middleware'ini en başa taşı
app.UseCors();

//app.UseHttpsRedirection();

app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async context =>
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "application/json";

		var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
		if (error != null)
		{
			var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
			logger.LogError(error.Error, "An unhandled exception occurred.");

			var result = JsonSerializer.Serialize(new
			{
				error = "An error occurred while processing your request.",
				details = error.Error.Message,
				stackTrace = error.Error.StackTrace
			});
			await context.Response.WriteAsync(result);
		}
	});
});
//app.UseHttpLogging();

// Authentication ve Authorization middleware'lerini CORS'tan sonra yerleştir
app.UseAuthentication();
app.UseAuthorization();
//app.UseResponseCaching();
app.MapControllers();

app.Run();
