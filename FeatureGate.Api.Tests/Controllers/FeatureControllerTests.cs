using FeatureGate.Api.Controllers;
using FeatureGate.Application.DTOs.Features;
using FeatureGate.Application.Interfaces.Services.Features;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FeatureGate.Tests.Controllers
{
    public class FeatureControllerTests
    {
        private readonly Mock<IFeatureService> _featureServiceMock;
        private readonly FeatureController _controller;

        public FeatureControllerTests()
        {
            _featureServiceMock = new Mock<IFeatureService>();
            _controller = new FeatureController(_featureServiceMock.Object);
        }

        // ============================
        // GetById
        // ============================

        [Fact]
        public async Task GetById_WhenFeatureExists_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new FeatureDto { Key = "feature-a" };

            _featureServiceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetById_WhenFeatureDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _featureServiceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((FeatureDto?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // ============================
        // GetAll
        // ============================

        [Fact]
        public async Task GetAll_ReturnsOkWithFeatures()
        {
            // Arrange
            var features = new List<FeatureDto>
            {
                new FeatureDto { Key = "a" },
                new FeatureDto { Key = "b" }
            };

            _featureServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(features);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(features);
        }

        // ============================
        // Create
        // ============================

        [Fact]
        public async Task Create_ReturnsOkWithCreatedFeature()
        {
            // Arrange
            var dto = new FeatureDto { Key = "new-feature" };

            _featureServiceMock
                .Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(dto);
        }

        // ============================
        // Update
        // ============================

        [Fact]
        public async Task Update_ReturnsOkWithUpdatedFeature()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new FeatureDto { Key = "updated-feature" };

            _featureServiceMock
                .Setup(s => s.UpdateAsync(id, dto))
                .ReturnsAsync(dto);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(dto);
        }

        // ============================
        // Delete
        // ============================

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            _featureServiceMock
                .Setup(s => s.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
