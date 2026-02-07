using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Rules;
using FeatureGate.Application.Rules.Engines;
using FluentAssertions;
using Moq;

namespace FeatureGate.Tests.Rules
{
    public class FeatureRuleEngineTests
    {
        private static FeatureEvaluationContext CreateContext()
        {
            return new FeatureEvaluationContext();
        }

        [Fact]
        public void Evaluate_WhenSingleRuleAppliesAndEvaluates_ReturnsResult()
        {
            // Arrange
            var context = CreateContext();

            var rule = new Mock<IFeatureRule>();

            var expectedResult = true;
            var evaluatedBy = "UserOverrideRule";

            rule.Setup(r => r.CanApply(context))
                .Returns(true);

            rule.Setup(r => r.TryEvaluate(context, out expectedResult, out evaluatedBy))
                .Returns(true);

            var engine = new FeatureRuleEngine(new[] { rule.Object });

            // Act
            var result = engine.Evaluate(context);

            // Assert
            result.Result.Should().BeTrue();
            result.EvaluatedBy.Should().Be("UserOverrideRule");
        }

        [Fact]
        public void Evaluate_WhenRuleCannotApply_ShouldSkipRuleAndThrow()
        {
            // Arrange
            var context = CreateContext();

            var rule = new Mock<IFeatureRule>();
            rule.Setup(r => r.CanApply(context)).Returns(false);

            var engine = new FeatureRuleEngine(new[] { rule.Object });

            // Act
            var action = () => engine.Evaluate(context);

            // Assert
            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("No rule applied.");
        }

        [Fact]
        public void Evaluate_WhenFirstRuleApplies_ShouldNotEvaluateRemainingRules()
        {
            // Arrange
            var context = CreateContext();

            var firstRule = new Mock<IFeatureRule>();
            var resultValue = false;
            var evaluatedBy = "FirstRule";

            firstRule.Setup(r => r.CanApply(context)).Returns(true);
            firstRule.Setup(r => r.TryEvaluate(context, out resultValue, out evaluatedBy))
                     .Returns(true);

            var secondRule = new Mock<IFeatureRule>();

            var engine = new FeatureRuleEngine(new[]
            {
                firstRule.Object,
                secondRule.Object
            });

            // Act
            var result = engine.Evaluate(context);

            // Assert
            result.EvaluatedBy.Should().Be("FirstRule");

            secondRule.Verify(
                r => r.CanApply(It.IsAny<FeatureEvaluationContext>()),
                Times.Never);
        }

        [Fact]
        public void Evaluate_WhenRuleAppliesButTryEvaluateFails_ShouldContinueToNextRule()
        {
            // Arrange
            var context = CreateContext();

            var firstRule = new Mock<IFeatureRule>();
            var dummyResult = false;
            var dummyBy = string.Empty;

            firstRule.Setup(r => r.CanApply(context)).Returns(true);
            firstRule.Setup(r => r.TryEvaluate(context, out dummyResult, out dummyBy))
                     .Returns(false);

            var secondRule = new Mock<IFeatureRule>();
            var finalResult = true;
            var evaluatedBy = "FallbackRule";

            secondRule.Setup(r => r.CanApply(context)).Returns(true);
            secondRule.Setup(r => r.TryEvaluate(context, out finalResult, out evaluatedBy))
                      .Returns(true);

            var engine = new FeatureRuleEngine(new[]
            {
                firstRule.Object,
                secondRule.Object
            });

            // Act
            var result = engine.Evaluate(context);

            // Assert
            result.Result.Should().BeTrue();
            result.EvaluatedBy.Should().Be("FallbackRule");
        }

        [Fact]
        public void Evaluate_WhenNoRuleApplies_ShouldThrowException()
        {
            // Arrange
            var context = CreateContext();

            var rule1 = new Mock<IFeatureRule>();
            var rule2 = new Mock<IFeatureRule>();

            rule1.Setup(r => r.CanApply(context)).Returns(false);
            rule2.Setup(r => r.CanApply(context)).Returns(false);

            var engine = new FeatureRuleEngine(new[]
            {
                rule1.Object,
                rule2.Object
            });

            // Act
            var action = () => engine.Evaluate(context);

            // Assert
            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("No rule applied.");
        }
    }
}
