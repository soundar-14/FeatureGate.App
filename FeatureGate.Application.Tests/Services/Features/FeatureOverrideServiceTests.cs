using AutoMapper;
using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Helpers.Constants;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Services.Features;
using FeatureGate.Domain.Entities;
using FluentAssertions;
using Moq;

namespace FeatureGate.Tests.Services.Features
{
    public class FeatureOverrideServiceTests
    {
        private readonly Mock<IFeatureOverrideRepository> _repositoryMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly FeatureOverrideService _service;

        public FeatureOverrideServiceTests()
        {
            _repositoryMock = new Mock<IFeatureOverrideRepository>();
            _cacheMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();

            _service = new FeatureOverrideService(
                _repositoryMock.Object,
                _cacheMock.Object,
                _mapperMock.Object);
        }

        // ============================
        // CreateAsync
        // ============================

        [Fact]
        public async Task CreateAsync_WhenValid_CreatesOverrideAndClearsCache()
        {
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "user-1"
            };

            var entity = new FeatureOverride
            {
                Id = Guid.NewGuid(),
                FeatureId = dto.FeatureId,
                TargetId = dto.TargetId
            };

            _mapperMock.Setup(m => m.Map<FeatureOverride>(dto))
                       .Returns(entity);

            _mapperMock.Setup(m => m.Map<FeatureOverrideDto>(entity))
                       .Returns(dto);

            var result = await _service.CreateAsync(dto);

            result.Should().NotBeNull();

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FeatureOverride>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _cacheMock.Verify(c =>
                c.RemoveByPrefixAsync(CacheKeys.FeatureAllPrefix),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenFeatureIdMissing_ThrowsException()
        {
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.Empty,
                TargetId = "user-1"
            };

            var action = () => _service.CreateAsync(dto);

            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("FeatureId is required");
        }

        [Fact]
        public async Task CreateAsync_WhenTargetIdMissing_ThrowsException()
        {
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = ""
            };

            var action = () => _service.CreateAsync(dto);

            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("TargetId is required");
        }

        // ============================
        // UpdateAsync
        // ============================

        [Fact]
        public async Task UpdateAsync_WhenValid_UpdatesOverrideAndClearsCache()
        {
            var id = Guid.NewGuid();

            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "group-1"
            };

            var entity = new FeatureOverride
            {
                Id = id,
                FeatureId = dto.FeatureId,
                TargetId = dto.TargetId
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                           .ReturnsAsync(entity);

            _mapperMock.Setup(m => m.Map(dto, entity));
            _mapperMock.Setup(m => m.Map<FeatureOverrideDto>(entity))
                       .Returns(dto);

            var result = await _service.UpdateAsync(id, dto);

            result.Should().NotBeNull();

            _repositoryMock.Verify(r => r.Update(entity), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _cacheMock.Verify(c =>
                c.RemoveByPrefixAsync(CacheKeys.FeatureAllPrefix),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenEntityNotFound_ThrowsException()
        {
            var id = Guid.NewGuid();
            var dto = new FeatureOverrideDto
            {
                FeatureId = Guid.NewGuid(),
                TargetId = "user"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                           .ReturnsAsync((FeatureOverride?)null);

            var action = () => _service.UpdateAsync(id, dto);

            await action.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Entity not found");
        }

        // ============================
        // DeleteAsync
        // ============================

        [Fact]
        public async Task DeleteAsync_WhenExists_DeletesOverrideAndClearsCache()
        {
            var id = Guid.NewGuid();
            var entity = new FeatureOverride { Id = id };

            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                           .ReturnsAsync(entity);

            await _service.DeleteAsync(id);

            _repositoryMock.Verify(r => r.Remove(entity), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _cacheMock.Verify(c =>
                c.RemoveByPrefixAsync(CacheKeys.FeatureAllPrefix),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsException()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(id))
                           .ReturnsAsync((FeatureOverride?)null);

            var action = () => _service.DeleteAsync(id);

            await action.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Entity not found");
        }
    }
}
