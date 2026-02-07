using AutoMapper;
using FeatureGate.Application.DTOs.Features;
using FeatureGate.Application.Helpers.Constants;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Services.Features;
using FeatureGate.Domain.Entities;
using FluentAssertions;
using Moq;

namespace FeatureGate.Tests.Services.Features
{
    public class FeatureServiceTests
    {
        private readonly Mock<IFeatureRepository> _featureRepoMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly FeatureService _service;

        public FeatureServiceTests()
        {
            _featureRepoMock = new Mock<IFeatureRepository>();
            _cacheMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();

            _service = new FeatureService(
                _featureRepoMock.Object,
                _mapperMock.Object,
                _cacheMock.Object);
        }

        // ============================
        // GetByIdAsync
        // ============================

        [Fact]
        public async Task GetByIdAsync_WhenEntityExists_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var entity = new Feature { Id = id, Key = "test" };
            var dto = new FeatureDto { Key = "test" };

            _featureRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<FeatureDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Key.Should().Be("test");
        }

        // ============================
        // GetAllAsync
        // ============================

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var entities = new List<Feature>
            {
                new Feature { Key = "a" },
                new Feature { Key = "b" }
            };

            var dtos = new List<FeatureDto>
            {
                new FeatureDto { Key = "a" },
                new FeatureDto { Key = "b" }
            };

            _featureRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<IList<FeatureDto>>(entities)).Returns(dtos);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        // ============================
        // CreateAsync
        // ============================

        [Fact]
        public async Task CreateAsync_WhenKeyIsUnique_CreatesFeature()
        {
            var dto = new FeatureDto { Key = "NewFeature" };
            var entity = new Feature { Key = "newfeature" };

            _featureRepoMock.Setup(r => r.GetByKeyAsync("newfeature"))
                            .ReturnsAsync((Feature?)null);

            _mapperMock.Setup(m => m.Map<Feature>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<FeatureDto>(It.IsAny<Feature>()))
                       .Returns(dto);

            var result = await _service.CreateAsync(dto);

            result.Key.Should().Be("newfeature");

            _featureRepoMock.Verify(r => r.AddAsync(It.IsAny<Feature>()), Times.Once);
            _featureRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenKeyExists_ThrowsException()
        {
            var dto = new FeatureDto { Key = "FeatureX" };

            _featureRepoMock.Setup(r => r.GetByKeyAsync("featurex"))
                            .ReturnsAsync(new Feature());

            var action = () => _service.CreateAsync(dto);

            await action.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Feature key already exists");
        }

        // ============================
        // UpdateAsync
        // ============================

        [Fact]
        public async Task UpdateAsync_WhenValid_UpdatesAndClearsCache()
        {
            var id = Guid.NewGuid();
            var dto = new FeatureDto { Key = "UpdatedFeature" };

            var entity = new Feature
            {
                Id = id,
                Key = "updatedfeature"
            };

            _featureRepoMock.Setup(r => r.GetByKeyAsync("updatedfeature"))
                            .ReturnsAsync((Feature?)null);

            _featureRepoMock.Setup(r => r.GetByIdAsync(id))
                            .ReturnsAsync(entity);

            _mapperMock.Setup(m => m.Map(dto, entity));
            _mapperMock.Setup(m => m.Map<FeatureDto>(entity))
                       .Returns(dto);

            var result = await _service.UpdateAsync(id, dto);

            result.Key.Should().Be("updatedfeature");

            _featureRepoMock.Verify(r => r.Update(entity), Times.Once);
            _featureRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _cacheMock.Verify(c =>
                c.RemoveByPrefixAsync(CacheKeys.FeaturePrefix("updatedfeature")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenDuplicateKeyExists_ThrowsException()
        {
            var id = Guid.NewGuid();
            var dto = new FeatureDto { Key = "Duplicate" };

            _featureRepoMock.Setup(r => r.GetByKeyAsync("duplicate"))
                            .ReturnsAsync(new Feature { Id = Guid.NewGuid() });

            var action = () => _service.UpdateAsync(id, dto);

            await action.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Feature key already exists");
        }

        // ============================
        // DeleteAsync
        // ============================

        [Fact]
        public async Task DeleteAsync_WhenExists_DeletesAndClearsCache()
        {
            var id = Guid.NewGuid();
            var entity = new Feature { Id = id, Key = "deletefeature" };

            _featureRepoMock.Setup(r => r.GetByIdAsync(id))
                            .ReturnsAsync(entity);

            await _service.DeleteAsync(id);

            _featureRepoMock.Verify(r => r.Remove(entity), Times.Once);
            _featureRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            _cacheMock.Verify(c =>
                c.RemoveByPrefixAsync(CacheKeys.FeaturePrefix("deletefeature")),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsException()
        {
            var id = Guid.NewGuid();

            _featureRepoMock.Setup(r => r.GetByIdAsync(id))
                            .ReturnsAsync((Feature?)null);

            var action = () => _service.DeleteAsync(id);

            await action.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Feature not found");
        }
    }
}
