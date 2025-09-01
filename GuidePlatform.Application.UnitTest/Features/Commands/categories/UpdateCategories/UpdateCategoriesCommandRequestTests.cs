using Microsoft.Extensions.Logging;
using Moq;
using GuidePlatform.Application.Features.Commands.categories.UpdateCategories;
using GuidePlatform.Domain.Entities;
using Karmed.External.Auth.Library.Services;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace GuidePlatform.Application.UnitTest.Features.Commands.categories.UpdateCategories
{
    public class UpdateCategoriesCommandRequestTests
    {
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        public UpdateCategoriesCommandRequestTests()
        {
            _mockCurrentUserService = new Mock<ICurrentUserService>();
        }

        [Fact]
        public void Map_ValidRequest_ShouldMapCorrectlyWithTokenUpdateUserId()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var originalUserId = Guid.NewGuid();
            var originalCustomerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();
            var updateUserId = Guid.NewGuid(); // 🎯 Güncelleme yapan kullanıcı - Güncelleme yapan kullanıcı

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = originalUserId,
                AuthCustomerId = originalCustomerId,
                CreateUserId = originalCreateUserId, // 🎯 Orijinal CreateUserId - Orijinal CreateUserId
                UpdateUserId = originalUserId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(updateUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(originalCustomerId);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Yeni Kategori", result.Name);
            Assert.Equal("Yeni açıklama", result.Description);
            Assert.Equal(updateUserId, result.AuthUserId);
            Assert.Equal(originalCustomerId, result.AuthCustomerId);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli - CreateUserId değişmemeli
            Assert.Equal(updateUserId, result.UpdateUserId); // 🎯 UpdateUserId token'dan alınmalı - UpdateUserId token'dan alınmalı
        }

        [Fact]
        public void Map_RequestWithNullDescription_ShouldSetDescriptionToNull()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = originalCreateUserId,
                UpdateUserId = userId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = null
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Null(result.Description);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli
        }

        [Fact]
        public void Map_RequestWithEmptyDescription_ShouldSetDescriptionToNull()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = originalCreateUserId,
                UpdateUserId = userId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = ""
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Null(result.Description);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli
        }

        [Fact]
        public void Map_RequestWithWhitespaceDescription_ShouldSetDescriptionToNull()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = originalCreateUserId,
                UpdateUserId = userId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = "   "
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Null(result.Description);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli
        }

        [Fact]
        public void Map_RequestWithTrimmedValues_ShouldTrimCorrectly()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = originalCreateUserId,
                UpdateUserId = userId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "  Yeni Kategori  ",
                Description = "  Yeni açıklama  "
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal("Yeni Kategori", result.Name);
            Assert.Equal("Yeni açıklama", result.Description);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli
        }

        [Fact]
        public void Map_RequestWithNullUserIds_ShouldNotUpdateUserIds()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var originalUserId = Guid.NewGuid();
            var originalCustomerId = Guid.NewGuid();
            var originalCreateUserId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = originalUserId,
                AuthCustomerId = originalCustomerId,
                CreateUserId = originalCreateUserId,
                UpdateUserId = originalUserId,
                RowCreatedDate = DateTime.UtcNow.AddDays(-1),
                RowUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RowIsActive = true,
                RowIsDeleted = false
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((Guid?)null);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns((Guid?)null);

            // 🎯 Act - Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.Equal(originalUserId, result.AuthUserId);
            Assert.Equal(originalCustomerId, result.AuthCustomerId);
            Assert.Equal(originalCreateUserId, result.CreateUserId); // 🎯 CreateUserId değişmemeli
            Assert.Equal(originalUserId, result.UpdateUserId); // 🎯 UpdateUserId de değişmemeli
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-guid")]
        public void Id_InvalidValue_ShouldFailValidation(string invalidId)
        {
            // 🎯 Arrange - Arrange
            var request = new UpdateCategoriesCommandRequest
            {
                Id = invalidId,
                Name = "Test Kategori",
                Description = "Test açıklama"
            };

            // 🎯 Act & Assert - Act & Assert
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Id"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Name_InvalidValue_ShouldFailValidation(string invalidName)
        {
            // 🎯 Arrange - Arrange
            var request = new UpdateCategoriesCommandRequest
            {
                Id = Guid.NewGuid().ToString(),
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
        public void Name_TooLong_ShouldFailValidation()
        {
            // 🎯 Arrange - Arrange
            var request = new UpdateCategoriesCommandRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = new string('a', 256), // 256 karakter (255'ten fazla)
                Description = "Test açıklama"
            };

            // 🎯 Act & Assert - Act & Assert
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Description_TooLong_ShouldFailValidation()
        {
            // 🎯 Arrange - Arrange
            var request = new UpdateCategoriesCommandRequest
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Kategori",
                Description = new string('a', 1001) // 1001 karakter (1000'den fazla)
            };

            // 🎯 Act & Assert - Act & Assert
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Description"));
        }

        [Fact]
        public void Map_WithValidToken_ShouldAutoFillUpdateUserId()
        {
            // Arrange - Test verilerini hazırla
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var existingCategory = new CategoriesViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Description = "Old Description",
                CreateUserId = Guid.NewGuid(), // 🎯 Mevcut CreateUserId korunacak
                UpdateUserId = Guid.NewGuid()  // 🎯 Bu güncellenecek
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = existingCategory.Id.ToString(),
                Name = "Updated Category",
                Description = "Updated Description"
                // 🎯 UpdateUserId boş bırakıldı - otomatik doldurulacak
            };

            // Act - Test method'unu çalıştır
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // Assert - Sonuçları kontrol et
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.Name);
            Assert.Equal("Updated Description", result.Description);
            
            // 🎯 Auth bilgileri token'dan otomatik alındı
            Assert.Equal(Guid.Parse(testCustomerId), result.AuthCustomerId);
            Assert.Equal(Guid.Parse(testUserId), result.AuthUserId);
            
            // 🎯 UpdateUserId otomatik token'dan alındı
            Assert.Equal(Guid.Parse(testUserId), result.UpdateUserId);
            
            // 🎯 CreateUserId değişmedi (korundu)
            Assert.Equal(existingCategory.CreateUserId, result.CreateUserId);
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

            var existingCategory = new CategoriesViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Description = "Old Description",
                CreateUserId = Guid.NewGuid(),
                UpdateUserId = Guid.NewGuid()
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = existingCategory.Id.ToString(),
                Name = "Updated Category",
                Description = "Updated Description",
                UpdateUserId = manualUpdateUserId // 🎯 Manuel UpdateUserId
            };

            // Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(manualUpdateUserId, result.UpdateUserId); // 🎯 Manuel değer kullanıldı
        }

        [Fact]
        public void Map_WithInvalidToken_ShouldHandleGracefully()
        {
            // Arrange - Geçersiz token ile test
            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns((string?)null);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns((string?)null);

            var existingCategory = new CategoriesViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Description = "Old Description",
                CreateUserId = Guid.NewGuid(),
                UpdateUserId = Guid.NewGuid()
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = existingCategory.Id.ToString(),
                Name = "Updated Category",
                Description = "Updated Description"
            };

            // Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.Name);
            Assert.Equal("Updated Description", result.Description);
            
            // 🎯 Geçersiz token durumunda null değerler
            Assert.Null(result.AuthCustomerId);
            Assert.Null(result.AuthUserId);
            Assert.Null(result.UpdateUserId);
            
            // 🎯 CreateUserId korundu
            Assert.Equal(existingCategory.CreateUserId, result.CreateUserId);
        }

        [Fact]
        public void Map_WithEmptyDescription_ShouldSetDescriptionToNull()
        {
            // Arrange
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var existingCategory = new CategoriesViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Description = "Old Description",
                CreateUserId = Guid.NewGuid(),
                UpdateUserId = Guid.NewGuid()
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = existingCategory.Id.ToString(),
                Name = "Updated Category",
                Description = "" // 🎯 Boş string
            };

            // Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.Name);
            Assert.Null(result.Description); // 🎯 Boş string null'a çevrildi
        }

        [Fact]
        public void Map_WithWhitespaceDescription_ShouldTrimDescription()
        {
            // Arrange
            var testUserId = "19a8b428-a57e-4a24-98e3-470258d3d83e";
            var testCustomerId = "72c54b1a-8e1c-45ea-8edd-b5da1091e325";

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(testUserId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(testCustomerId);

            var existingCategory = new CategoriesViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Description = "Old Description",
                CreateUserId = Guid.NewGuid(),
                UpdateUserId = Guid.NewGuid()
            };

            var request = new UpdateCategoriesCommandRequest
            {
                Id = existingCategory.Id.ToString(),
                Name = "Updated Category",
                Description = "  Trimmed Description  " // 🎯 Boşluklu string
            };

            // Act
            var result = UpdateCategoriesCommandRequest.Map(existingCategory, request, _mockCurrentUserService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Category", result.Name);
            Assert.Equal("Trimmed Description", result.Description); // 🎯 Boşluklar temizlendi
        }
    }
}
