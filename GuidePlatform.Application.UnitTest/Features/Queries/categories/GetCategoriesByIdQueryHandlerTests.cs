using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Dtos.Response;
using GuidePlatform.Application.Features.Queries.categories.GetCategoriesById;
using GuidePlatform.Application.Services;
using GuidePlatform.Application.Abstractions.Services;
using GuidePlatform.Domain.Entities.categories;
using Moq;
using Xunit;

namespace GuidePlatform.Application.UnitTest.Features.Queries.categories
{
    /// <summary>
    /// GetCategoriesByIdQueryHandler için Unit Tests - Auth filtrelerinin doğru çalıştığını test eder
    /// </summary>
    public class GetCategoriesByIdQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<IAuthUserDetailService> _mockAuthUserService;
        private readonly CurrentUserService _currentUserService;
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<DbSet<categories>> _mockCategoriesDbSet;

        public GetCategoriesByIdQueryHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockAuthUserService = new Mock<IAuthUserDetailService>();
            _mockCategoriesDbSet = new Mock<DbSet<categories>>();

            // HttpContext setup
            _httpContext = new DefaultHttpContext();
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = _httpContext
            };
            _currentUserService = new CurrentUserService(httpContextAccessor);

            // Mock DbSet setup
            _mockContext.Setup(x => x.categories).Returns(_mockCategoriesDbSet.Object);
        }

        [Fact]
        public async Task Handle_WithValidIdAndMatchingAuthData_ShouldReturnSuccess()
        {
            // Arrange
            var categoriesId = Guid.Parse("8c90c2e0-329d-4a11-89ea-6b18f06502a4");
            var authUserId = Guid.Parse("19a8b428-a57e-4a24-98e3-470258d3d83e");
            var authCustomerId = Guid.Parse("72c54b1a-8e1c-45ea-8edd-b5da1091e325");

            // Setup JWT claims
            var userData = new ClaimUserDataDto
            {
                UserId = authUserId.ToString(),
                CustomerId = authCustomerId.ToString()
            };
            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", userDataJson)
            };
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Setup test data
            var testCategories = new categories
            {
                Id = categoriesId,
                AuthUserId = authUserId,
                AuthCustomerId = authCustomerId,
                Name = "Test Category",
                Description = "Test Description",
                RowIsActive = true,
                RowIsDeleted = false,
                RowCreatedDate = DateTime.UtcNow,
                RowUpdatedDate = DateTime.UtcNow
            };

            var queryableData = new List<categories> { testCategories }.AsQueryable();
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            // Setup auth user service
            _mockAuthUserService.Setup(x => x.GetAuthUserDetailAsync(authUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AuthUserDetailDto
                {
                    AuthUserId = authUserId,
                    AuthUserName = "Test User",
                    AuthCustomerName = "Test Customer"
                });

            var handler = new GetCategoriesByIdQueryHandler(
                _mockContext.Object,
                _mockAuthUserService.Object,
                _currentUserService);

            var request = new GetCategoriesByIdQueryRequest
            {
                Id = categoriesId.ToString()
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.OperationStatus);
            Assert.NotNull(result.Result);
            Assert.NotNull(result.Result.categories);
            Assert.Equal(categoriesId, result.Result.categories.Id);
            Assert.Equal(authUserId, result.Result.categories.AuthUserId);
            Assert.Equal(authCustomerId, result.Result.categories.AuthCustomerId);
            Assert.Equal("Test User", result.Result.categories.AuthUserName);
            Assert.Equal("Test Customer", result.Result.categories.AuthCustomerName);
        }

        [Fact]
        public async Task Handle_WithValidIdButNoMatchingAuthData_ShouldReturnError()
        {
            // Arrange
            var categoriesId = Guid.Parse("8c90c2e0-329d-4a11-89ea-6b18f06502a4");
            var authUserId = Guid.Parse("19a8b428-a57e-4a24-98e3-470258d3d83e");
            var authCustomerId = Guid.Parse("72c54b1a-8e1c-45ea-8edd-b5da1091e325");

            // Setup JWT claims
            var userData = new ClaimUserDataDto
            {
                UserId = authUserId.ToString(),
                CustomerId = authCustomerId.ToString()
            };
            var userDataJson = JsonSerializer.Serialize(userData);
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/userdata", userDataJson)
            };
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            // Setup test data with different auth data
            var testCategories = new categories
            {
                Id = categoriesId,
                AuthUserId = Guid.NewGuid(), // Different user
                AuthCustomerId = Guid.NewGuid(), // Different customer
                Name = "Test Category",
                Description = "Test Description",
                RowIsActive = true,
                RowIsDeleted = false,
                RowCreatedDate = DateTime.UtcNow,
                RowUpdatedDate = DateTime.UtcNow
            };

            var queryableData = new List<categories> { testCategories }.AsQueryable();
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockCategoriesDbSet.As<IQueryable<categories>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            var handler = new GetCategoriesByIdQueryHandler(
                _mockContext.Object,
                _mockAuthUserService.Object,
                _currentUserService);

            var request = new GetCategoriesByIdQueryRequest
            {
                Id = categoriesId.ToString()
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.OperationStatus);
            Assert.Null(result.Result);
            Assert.Contains("categories Bulunamadı", result.OperationResult.MessageTitle);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var handler = new GetCategoriesByIdQueryHandler(
                _mockContext.Object,
                _mockAuthUserService.Object,
                _currentUserService);

            var request = new GetCategoriesByIdQueryRequest
            {
                Id = "invalid-guid"
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.OperationStatus);
            Assert.Null(result.Result);
            Assert.Contains("Geçersiz ID", result.OperationResult.MessageTitle);
        }

        [Fact]
        public async Task Handle_WithEmptyId_ShouldReturnError()
        {
            // Arrange
            var handler = new GetCategoriesByIdQueryHandler(
                _mockContext.Object,
                _mockAuthUserService.Object,
                _currentUserService);

            var request = new GetCategoriesByIdQueryRequest
            {
                Id = ""
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.OperationStatus);
            Assert.Null(result.Result);
            Assert.Contains("Eksik Parametre", result.OperationResult.MessageTitle);
        }

        [Fact]
        public async Task Handle_WithNullId_ShouldReturnError()
        {
            // Arrange
            var handler = new GetCategoriesByIdQueryHandler(
                _mockContext.Object,
                _mockAuthUserService.Object,
                _currentUserService);

            var request = new GetCategoriesByIdQueryRequest
            {
                Id = null
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.OperationStatus);
            Assert.Null(result.Result);
            Assert.Contains("Eksik Parametre", result.OperationResult.MessageTitle);
        }
    }
}
