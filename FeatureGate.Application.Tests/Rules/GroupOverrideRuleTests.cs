using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Rules;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FluentAssertions;

namespace FeatureGate.Tests.Rules
{
    public class GroupOverrideRuleTests
    {
        private FeatureEvaluationContext CreateContext()
        {
            return new FeatureEvaluationContext
            {
                GroupId = "admins",
                Feature = new Feature { DefaultState = true }
            };
        }

        [Fact]
        public void CanApply_WhenGroupIdExists_ReturnsTrue()
        {
            var rule = new GroupOverrideRule();
            var context = CreateContext();

            rule.CanApply(context).Should().BeTrue();
        }

        [Fact]
        public void TryEvaluate_WhenGroupOverrideExists_ReturnsOverrideValue()
        {
            var rule = new GroupOverrideRule();
            var context = CreateContext();

            context.Overrides.Add(new FeatureOverride
            {
                TargetType = OverrideType.Group,
                TargetId = "admins",
                State = false
            });

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeTrue();
            result.Should().BeFalse();
            by.Should().Be("GroupOverride");
        }

        [Fact]
        public void TryEvaluate_WhenGroupOverrideNotFound_ReturnsFalse()
        {
            var rule = new GroupOverrideRule();
            var context = CreateContext();

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeFalse();
            result.Should().BeFalse();
            by.Should().BeEmpty();
        }
    }
}
