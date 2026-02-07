using FeatureGate.Api.Controllers;
using FeatureGate.Application.DTOs.Evaluations;
using FeatureGate.Application.Interfaces.Services.Features;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FeatureGate.Tests.Controllers
{
    public class FeatureEvaluationControllerTests
    {
        private readonly Mock<IFeatureEvaluationService> _evaluationServiceMock;
        private readonly FeatureEvaluationController _controller;

        public FeatureEvaluationControllerTests()
        {
            _evaluationServiceMock = new Mock<IFeatureEvaluationService>();
            _controller = new FeatureEvaluationController(_evaluationServiceMock.Object);
        }

        [Fact]
        public async Task Evaluate_ReturnsOkWithEvaluationResult()
        {
            // Arrange
            var request = new FeatureEvaluationRequestDto
            {
                FeatureKey = "feature-x",
                UserId = "user-1",
                GroupId = "admins",
                Region = "IN"
            };

            var resultDto = new FeatureEvaluationResultDto
            {
                FeatureKey = "feature-x",
                IsEnabled = true,
                EvaluatedBy = "UserOverride"
            };

            _evaluationServiceMock
                .Setup(s => s.EvaluateAsync(request))
                .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.Evaluate(request);

            // Assert
            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeEquivalentTo(resultDto);

            _evaluationServiceMock.Verify(
                s => s.EvaluateAsync(request),
                Times.Once);
        }
    }
}
