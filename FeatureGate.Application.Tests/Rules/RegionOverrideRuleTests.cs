using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Rules;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;
using FluentAssertions;

namespace FeatureGate.Tests.Rules
{
    public class RegionOverrideRuleTests
    {
        private FeatureEvaluationContext CreateContext()
        {
            return new FeatureEvaluationContext
            {
                Region = "IN",
                Feature = new Feature { DefaultState = true }
            };
        }

        [Fact]
        public void CanApply_WhenRegionExists_ReturnsTrue()
        {
            var rule = new RegionOverrideRule();
            var context = CreateContext();

            rule.CanApply(context).Should().BeTrue();
        }

        [Fact]
        public void TryEvaluate_WhenRegionOverrideExists_ReturnsOverrideValue()
        {
            var rule = new RegionOverrideRule();
            var context = CreateContext();

            context.Overrides.Add(new FeatureOverride
            {
                TargetType = OverrideType.Region,
                TargetId = "IN",
                State = false
            });

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeTrue();
            result.Should().BeFalse();
            by.Should().Be("RegionOverride");
        }

        [Fact]
        public void TryEvaluate_WhenRegionOverrideNotFound_ReturnsFalse()
        {
            var rule = new RegionOverrideRule();
            var context = CreateContext();

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeFalse();
            result.Should().BeFalse();
            by.Should().BeEmpty();
        }
    }
}
