using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Rules;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FluentAssertions;

namespace FeatureGate.Tests.Rules
{
    public class UserOverrideRuleTests
    {
        private FeatureEvaluationContext CreateContext()
        {
            return new FeatureEvaluationContext
            {
                UserId = "user-1",
                Feature = new Feature { DefaultState = true }
            };
        }

        [Fact]
        public void CanApply_WhenUserIdExists_ReturnsTrue()
        {
            var rule = new UserOverrideRule();
            var context = CreateContext();

            rule.CanApply(context).Should().BeTrue();
        }

        [Fact]
        public void TryEvaluate_WhenUserOverrideExists_ReturnsOverrideValue()
        {
            var rule = new UserOverrideRule();
            var context = CreateContext();

            context.Overrides.Add(new FeatureOverride
            {
                TargetType = OverrideType.User,
                TargetId = "user-1",
                State = false
            });

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeTrue();
            result.Should().BeFalse();
            by.Should().Be("UserOverride");
        }

        [Fact]
        public void TryEvaluate_WhenUserOverrideNotFound_ReturnsFalse()
        {
            var rule = new UserOverrideRule();
            var context = CreateContext();

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeFalse();
            result.Should().BeFalse();
            by.Should().BeEmpty();
        }
    }
}
