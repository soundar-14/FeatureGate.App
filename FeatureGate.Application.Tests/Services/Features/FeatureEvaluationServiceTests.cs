using AutoMapper;
using FeatureGate.Application.DTOs.Evaluations;
using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Rules;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Rules.Engines;
using FeatureGate.Application.Services.Features;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FluentValidation;
using Moq;
using Xunit;

namespace FeatureGate.Tests.Services.Features
{
    public class FeatureEvaluationServiceTests
    {
        private readonly Mock<IFeatureRepository> _featureRepoMock;
        private readonly Mock<IFeatureOverrideRepository> _overrideRepoMock;
        private readonly Mock<IValidator<FeatureEvaluationRequestDto>> _validator;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheMock;

        private readonly FeatureEvaluationService _service;

        public FeatureEvaluationServiceTests()
        {
            _featureRepoMock = new Mock<IFeatureRepository>();
            _overrideRepoMock = new Mock<IFeatureOverrideRepository>();
            _validator = new Mock<IValidator<FeatureEvaluationRequestDto>>();
            _mapperMock = new Mock<IMapper>();
            _cacheMock = new Mock<ICacheService>();

            // ✅ Mock rule (NOT the engine)
            var ruleMock = new Mock<IFeatureRule>();

            ruleMock
                .Setup(r => r.CanApply(It.IsAny<FeatureEvaluationContext>()))
                .Returns(true);

            ruleMock
                .Setup(r => r.TryEvaluate(
                    It.IsAny<FeatureEvaluationContext>(),
                    out It.Ref<bool>.IsAny,
                    out It.Ref<string>.IsAny))
                .Returns((FeatureEvaluationContext ctx, out bool result, out string by) =>
                {
                    result = true;
                    by = "UserOverride";
                    return true;
                });

            var ruleEngine = new FeatureRuleEngine(new[] { ruleMock.Object });

            _service = new FeatureEvaluationService(
                _featureRepoMock.Object,
                _overrideRepoMock.Object,
                ruleEngine,
                _validator.Object,
                _mapperMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task EvaluateAsync_WhenCached_ReturnsCachedValue()
        {
            var request = new FeatureEvaluationRequestDto
            {
                FeatureKey = "test",
                UserId = "u1"
            };

            var cached = new FeatureEvaluationResultDto
            {
                FeatureKey = "test",
                IsEnabled = true,
                EvaluatedBy = "Cache"
            };

            _cacheMock
                .Setup(c => c.GetAsync<FeatureEvaluationResultDto>(It.IsAny<string>()))
                .ReturnsAsync(cached);

            var result = await _service.EvaluateAsync(request);

            Assert.True(result.IsEnabled);
            Assert.Equal("Cache", result.EvaluatedBy);

            _featureRepoMock.Verify(
                r => r.GetByKeyAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task EvaluateAsync_WhenFeatureNotFound_ThrowsException()
        {
            var request = new FeatureEvaluationRequestDto
            {
                FeatureKey = "missing"
            };

            _cacheMock
                .Setup(c => c.GetAsync<FeatureEvaluationResultDto>(It.IsAny<string>()))
                .ReturnsAsync((FeatureEvaluationResultDto?)null);

            _featureRepoMock
                .Setup(r => r.GetByKeyAsync(It.IsAny<string>()))
                .ReturnsAsync((Feature)null!);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.EvaluateAsync(request));
        }

        [Fact]
        public async Task EvaluateAsync_WhenEvaluated_ReturnsResultAndCaches()
        {
            var feature = new Feature
            {
                Id = Guid.NewGuid(),
                Key = "newfeature",
                DefaultState = false
            };

            var overrides = new[]
            {
                new FeatureOverride
                {
                    FeatureId = feature.Id,
                    TargetType = OverrideType.User,
                    TargetId = "u1",
                    State = true
                }
            };

            var request = new FeatureEvaluationRequestDto
            {
                FeatureKey = "newfeature",
                UserId = "u1"
            };

            var mappedResult = new FeatureEvaluationResultDto
            {
                IsEnabled = true,
                EvaluatedBy = "UserOverride"
            };

            _cacheMock
                .Setup(c => c.GetAsync<FeatureEvaluationResultDto>(It.IsAny<string>()))
                .ReturnsAsync((FeatureEvaluationResultDto?)null);

            _featureRepoMock
                .Setup(r => r.GetByKeyAsync("newfeature"))
                .ReturnsAsync(feature);

            _overrideRepoMock
                .Setup(r => r.GetByFeatureIdAsync(feature.Id))
                .ReturnsAsync(overrides);

            _mapperMock
                .Setup(m => m.Map<FeatureEvaluationResultDto>(It.IsAny<(bool, string)>()))
                .Returns(mappedResult);

            var result = await _service.EvaluateAsync(request);

            Assert.True(result.IsEnabled);
            Assert.Equal("UserOverride", result.EvaluatedBy);
            Assert.Equal("newfeature", result.FeatureKey);

            _cacheMock.Verify(
                c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<FeatureEvaluationResultDto>(),
                    It.IsAny<TimeSpan>()),
                Times.Once);
        }
    }
}
