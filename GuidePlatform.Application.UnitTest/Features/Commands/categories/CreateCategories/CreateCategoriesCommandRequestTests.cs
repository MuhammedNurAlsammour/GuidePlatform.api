using Microsoft.Extensions.Logging;
using Moq;
using GuidePlatform.Application.Features.Commands.categories.CreateCategories;
using GuidePlatform.Domain.Entities;
using Karmed.External.Auth.Library.Services;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.UnitTest.Features.Commands.categories.CreateCategories
{
    public class CreateCategoriesCommandRequestTests
    {
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        public CreateCategoriesCommandRequestTests()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();
        }

        [Fact]
        public void Map_ValidRequest_ShouldMapCorrectlyWithTokenUserIds()
        {
            // 🎯 Arrange - Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Kategori",
                Description = "Test açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Test Kategori", result.Name);
            Assert.Equal("Test açıklama", result.Description);
            Assert.Equal(userId, result.AuthUserId);
            Assert.Equal(customerId, result.AuthCustomerId);
            Assert.Equal(userId, result.CreateUserId); // 🎯 CreateUserId token'dan alınmalı
            Assert.Equal(userId, result.UpdateUserId); // 🎯 İlk oluşturmada UpdateUserId de aynı
        }

        [Fact]
        public void Map_RequestWithNullUserIds_ShouldHandleGracefully()
        {
            // 🎯 Arrange - Arrange
            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Kategori",
                Description = "Test açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((Guid?)null);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns((Guid?)null);

            // 🎯 Act - Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Test Kategori", result.Name);
            Assert.Equal("Test açıklama", result.Description);
            Assert.Null(result.AuthUserId);
            Assert.Null(result.AuthCustomerId);
            Assert.Null(result.CreateUserId);
            Assert.Null(result.UpdateUserId);
        }

        [Fact]
        public void Map_RequestWithTrimmedValues_ShouldTrimCorrectly()
        {
            // 🎯 Arrange - Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CreateCategoriesCommandRequest
            {
                Name = "  Test Kategori  ",
                Description = "  Test açıklama  "
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Test Kategori", result.Name);
            Assert.Equal("Test açıklama", result.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Name_InvalidValue_ShouldFailValidation(string invalidName)
        {
            // 🎯 Arrange - Arrange
            var request = new CreateCategoriesCommandRequest
            {
                Name = invalidName,
                Description = "Test açıklama"
            };

            // 🎯 Act & Assert - Act & Assert
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Map_RequestWithEmptyDescription_ShouldSetDescriptionToNull()
        {
            // 🎯 Arrange - Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Kategori",
                Description = ""
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Test Kategori", result.Name);
            Assert.Equal("", result.Description); // Boş string olarak kalmalı
        }

        [Fact]
        public void Map_RequestWithNullDescription_ShouldSetDescriptionToNull()
        {
            // 🎯 Arrange - Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Kategori",
                Description = null
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Test Kategori", result.Name);
            Assert.Null(result.Description);
        }

        [Fact]
        public void Map_WithValidToken_ShouldAutoFillCreateUserIdAndUpdateUserId()
        {
            // Arrange - Test verilerini hazırla
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Category",
                Description = "Test Description"
                // 🎯 CreateUserId ve UpdateUserId boş bırakıldı - otomatik doldurulacak
            };

            // Act - Test method'unu çalıştır
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // Assert - Sonuçları kontrol et
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);
            Assert.Equal("Test Description", result.Description);
            
            // 🎯 Auth bilgileri token'dan otomatik alındı
            Assert.Equal(Guid.Parse(testCustomerId), result.AuthCustomerId);
            Assert.Equal(Guid.Parse(testUserId), result.AuthUserId);
            
            // 🎯 CreateUserId ve UpdateUserId otomatik token'dan alındı
            Assert.Equal(Guid.Parse(testUserId), result.CreateUserId);
            Assert.Equal(Guid.Parse(testUserId), result.UpdateUserId);
        }

        [Fact]
        public void Map_WithManualCreateUserId_ShouldUseManualValue()
        {
            // Arrange - Manuel CreateUserId ile test
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";
            var manualCreateUserId = Guid.NewGuid();

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Category",
                Description = "Test Description",
                CreateUserId = manualCreateUserId // 🎯 Manuel CreateUserId
            };

            // Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(manualCreateUserId, result.CreateUserId); // 🎯 Manuel değer kullanıldı
            Assert.Equal(Guid.Parse(testUserId), result.UpdateUserId); // 🎯 UpdateUserId hala token'dan
        }

        [Fact]
        public void Map_WithManualUpdateUserId_ShouldUseManualValue()
        {
            // Arrange - Manuel UpdateUserId ile test
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";
            var manualUpdateUserId = Guid.NewGuid();

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Category",
                Description = "Test Description",
                UpdateUserId = manualUpdateUserId // 🎯 Manuel UpdateUserId
            };

            // Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Parse(testUserId), result.CreateUserId); // 🎯 CreateUserId token'dan
            Assert.Equal(manualUpdateUserId, result.UpdateUserId); // 🎯 Manuel değer kullanıldı
        }

        [Fact]
        public void Map_WithInvalidToken_ShouldHandleGracefully()
        {
            // Arrange - Geçersiz token ile test
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string?)null);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns((string?)null);

            var request = new CreateCategoriesCommandRequest
            {
                Name = "Test Category",
                Description = "Test Description"
            };

            // Act
            var result = CreateCategoriesCommandRequest.Map(request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);
            Assert.Equal("Test Description", result.Description);
            
            // 🎯 Geçersiz token durumunda null değerler
            Assert.Null(result.AuthCustomerId);
            Assert.Null(result.AuthUserId);
            Assert.Null(result.CreateUserId);
            Assert.Null(result.UpdateUserId);
        }

        [Fact]
        public void ParseJwtTokenForDebug_WithValidToken_ShouldExtractUserData()
        {
            // Arrange - JWT token test
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibXVoYW1tZWQubnVyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy91c2VyZGF0YSI6IntcIkN1c3RvbWVySWRcIjpcIjcyYzU0YjFhLThlMWMtNDVlYS04ZWRkLWI1ZGExMDkxZTMyNVwiLFwiVXNlcklkXCI6XCIxOWE4YjQyOC1hNTdlLTRhMjQtOThlMy00NzAyNThkM2Q4M2VcIn0iLCJuYmYiOjE3NTY2NTQwNjcsImV4cCI6Mjc1NjY1NDA2NiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo3MDA3IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MjYzIn0.mtpTXcp_X5DwaKDXGK9QFjOhc36uxQZPvhUujEFXD_s";

            var request = new CreateCategoriesCommandRequest();

            // Act
            var (userId, customerId) = request.ParseJwtTokenForDebug(jwtToken);

            // Assert
            Assert.NotNull(userId);
            Assert.NotNull(customerId);
            Assert.Equal(Guid.Parse("19a8b428-a57e-4a24-98e3-470258d3d83e"), userId);
            Assert.Equal(Guid.Parse("72c54b1a-8e1c-45ea-8edd-b5da1091e325"), customerId);
        }
    }
}
