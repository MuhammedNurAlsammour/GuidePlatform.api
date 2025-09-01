using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using GuidePlatform.Application.Abstractions.Contexts;
using GuidePlatform.Application.Features.Commands.categories.UpdateCategories;
using GuidePlatform.Domain.Entities;
using Karmed.External.Auth.Library.Services;
using GuidePlatform.Application.Operations;
using Xunit;

namespace GuidePlatform.Application.UnitTest.Features.Commands.categories.UpdateCategories
{
    public class UpdateCategoriesCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<ILogger<UpdateCategoriesCommandHandler>> _mockLogger;
        private readonly Mock<DbSet<CategoriesViewModel>> _mockDbSet;
        private readonly UpdateCategoriesCommandHandler _handler;

        public UpdateCategoriesCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<UpdateCategoriesCommandHandler>>();
            _mockDbSet = new Mock<DbSet<CategoriesViewModel>>();

            _handler = new UpdateCategoriesCommandHandler(
                _mockContext.Object,
                _mockCurrentUserService.Object,
                _mockLogger.Object
            );

            // 🎯 Mock setup - Mock setup
            _mockContext.Setup(x => x.categories).Returns(_mockDbSet.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdateCategorySuccessfully()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = userId,
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
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            var mockQueryable = new List<CategoriesViewModel> { existingCategory }.AsQueryable();
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());

            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // 🎯 Act - Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.True(result.OperationStatus);
            Assert.Equal("İşlem Başarılı", result.OperationResult.MessageTitle);
            Assert.Equal("categories başarıyla güncellendi.", result.OperationResult.MessageContent);

            _mockContext.Verify(x => x.categories.Update(It.IsAny<CategoriesViewModel>()), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ShouldReturnError()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new UpdateCategoriesCommandRequest
            {
                Id = categoryId.ToString(),
                Name = "Yeni Kategori",
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            var mockQueryable = new List<CategoriesViewModel>().AsQueryable();
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());

            // 🎯 Act - Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.False(result.OperationStatus);
            Assert.Equal("Hata / İşlem Başarısız", result.OperationResult.MessageTitle);
            Assert.Equal("Güncellenecek categories bulunamadı.", result.OperationResult.MessageContent);

            _mockContext.Verify(x => x.categories.Update(It.IsAny<CategoriesViewModel>()), Times.Never);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DatabaseException_ShouldReturnError()
        {
            // 🎯 Arrange - Arrange
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var existingCategory = new CategoriesViewModel
            {
                Id = categoryId,
                Name = "Eski Kategori",
                Description = "Eski açıklama",
                AuthUserId = userId,
                AuthCustomerId = customerId,
                CreateUserId = userId,
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
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            var mockQueryable = new List<CategoriesViewModel> { existingCategory }.AsQueryable();
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Provider).Returns(mockQueryable.Provider);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.Expression).Returns(mockQueryable.Expression);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.ElementType).Returns(mockQueryable.ElementType);
            _mockDbSet.As<IQueryable<CategoriesViewModel>>().Setup(m => m.GetEnumerator()).Returns(mockQueryable.GetEnumerator());

            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Database error"));

            // 🎯 Act - Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // 🎯 Assert - Assert
            Assert.NotNull(result);
            Assert.False(result.OperationStatus);
            Assert.Equal("Veritabanı Hatası", result.OperationResult.MessageTitle);
            Assert.Equal("Veritabanı güncelleme işlemi başarısız oldu.", result.OperationResult.MessageContent);
        }

        [Fact]
        public async Task Handle_InvalidGuid_ShouldReturnError()
        {
            // 🎯 Arrange - Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var request = new UpdateCategoriesCommandRequest
            {
                Id = "invalid-guid",
                Name = "Yeni Kategori",
                Description = "Yeni açıklama"
            };

            _mockCurrentUserService.Setup(x => x.GetUserId()).Returns(userId);
            _mockCurrentUserService.Setup(x => x.GetCustomerId()).Returns(customerId);

            // 🎯 Act & Assert - Act & Assert
            await Assert.ThrowsAsync<FormatException>(() => 
                _handler.Handle(request, CancellationToken.None));
        }
    }
}
