using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using GuidePlatform.Application.Services;
using GuidePlatform.Application.Dtos.Response;
using Xunit;

namespace GuidePlatform.Application.UnitTest.Services
{
    /// <summary>
    /// CurrentUserService için Unit Tests - JWT Token'dan veri çıkarma işlemlerini test eder
    /// </summary>
    public class CurrentUserServiceTests
    {
        private readonly CurrentUserService _currentUserService;
        private readonly DefaultHttpContext _httpContext;

        public CurrentUserServiceTests()
        {
            _httpContext = new DefaultHttpContext();
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = _httpContext
            };
            _currentUserService = new CurrentUserService(httpContextAccessor);
        }

        [Fact]
        public void GetUserId_WithValidUserDataClaim_ShouldReturnUserId()
        {
            // Arrange
            var userData = new ClaimUserDataDto
            {
                UserId = "19a8b428-a57e-4a24-98e3-470258d3d83e",
                CustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325"
            };

            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", userDataJson)
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetUserId();

            // Assert
            Assert.Equal("19a8b428-a57e-4a24-98e3-470258d3d83e", result);
        }

        [Fact]
        public void GetCustomerId_WithValidUserDataClaim_ShouldReturnCustomerId()
        {
            // Arrange
            var userData = new ClaimUserDataDto
            {
                UserId = "19a8b428-a57e-4a24-98e3-470258d3d83e",
                CustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325"
            };

            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", userDataJson)
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetCustomerId();

            // Assert
            Assert.Equal("72c54b1a-8e1c-45ea-8edd-b5da1091e325", result);
        }

        [Fact]
        public void GetUserId_WithFallbackClaimType_ShouldReturnUserId()
        {
            // Arrange
            var userData = new ClaimUserDataDto
            {
                UserId = "19a8b428-a57e-4a24-98e3-470258d3d83e",
                CustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325"
            };

            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.UserData, userDataJson)
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetUserId();

            // Assert
            Assert.Equal("19a8b428-a57e-4a24-98e3-470258d3d83e", result);
        }

        [Fact]
        public void GetCustomerId_WithFallbackClaimType_ShouldReturnCustomerId()
        {
            // Arrange
            var userData = new ClaimUserDataDto
            {
                UserId = "19a8b428-a57e-4a24-98e3-470258d3d83e",
                CustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325"
            };

            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.UserData, userDataJson)
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetCustomerId();

            // Assert
            Assert.Equal("72c54b1a-8e1c-45ea-8edd-b5da1091e325", result);
        }

        [Fact]
        public void GetUserId_WithInvalidJson_ShouldReturnNull()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", "invalid json")
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetUserId();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCustomerId_WithInvalidJson_ShouldReturnNull()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", "invalid json")
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetCustomerId();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserId_WithNoClaims_ShouldReturnNull()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = _currentUserService.GetUserId();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCustomerId_WithNoClaims_ShouldReturnNull()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = _currentUserService.GetCustomerId();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUsername_WithValidName_ShouldReturnUsername()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "muhammed.nur")
            };

            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Act
            var result = _currentUserService.GetUsername();

            // Assert
            Assert.Equal("muhammed.nur", result);
        }

        [Fact]
        public void GetUsername_WithNoName_ShouldReturnDefaultValue()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = _currentUserService.GetUsername();

            // Assert
            Assert.Equal("Unauthorised user", result);
        }
    }
}
