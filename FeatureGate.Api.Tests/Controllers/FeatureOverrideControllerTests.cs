using FeatureGate.Api.Controllers;
using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Interfaces.Services.Features;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FeatureGate.Tests.Controllers
{
    public class FeatureOverrideControllerTests
    {
        private readonly Mock<IFeatureOverrideService> _overrideServiceMock;
        private readonly FeatureOverrideController _controller;

        public FeatureOverrideControllerTests()
        {
            _overrideServiceMock = new Mock<IFeatureOverrideService>();
            _controller = new FeatureOverrideController(_overrideServiceMock.Object);
        }

        // ============================
        // GetById
        // ============================

        [Fact]
        public async Task GetById_WhenOverrideExists_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "user-1"
            };

            _overrideServiceMock
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
        public async Task GetById_WhenOverrideDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _overrideServiceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((FeatureOverrideDto?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // ============================
        // GetAll
        // ============================

        [Fact]
        public async Task GetAll_ReturnsOkWithOverrides()
        {
            // Arrange
            var list = new List<FeatureOverrideDto>
            {
                new FeatureOverrideDto { FeatureId = Guid.NewGuid(), TargetId = "u1" },
                new FeatureOverrideDto { FeatureId = Guid.NewGuid(), TargetId = "u2" }
            };

            _overrideServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(list);
        }

        // ============================
        // Create
        // ============================

        [Fact]
        public async Task Create_ReturnsOkWithCreatedOverride()
        {
            // Arrange
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "user-1"
            };

            _overrideServiceMock
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
        public async Task Update_ReturnsOkWithUpdatedOverride()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "group-1"
            };

            _overrideServiceMock
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

            _overrideServiceMock
                .Setup(s => s.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
