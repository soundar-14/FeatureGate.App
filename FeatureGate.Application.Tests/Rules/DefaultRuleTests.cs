using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Rules;
using FeatureGate.Domain.Entities;
using FluentAssertions;

namespace FeatureGate.Tests.Rules
{
    public class DefaultRuleTests
    {
        [Fact]
        public void CanApply_Always_ReturnsTrue()
        {
            var rule = new DefaultRule();
            var context = new FeatureEvaluationContext();

            rule.CanApply(context).Should().BeTrue();
        }

        [Fact]
        public void TryEvaluate_ReturnsFeatureDefaultState()
        {
            var rule = new DefaultRule();
            var context = new FeatureEvaluationContext
            {
                Feature = new Feature
                {
                    DefaultState = true
                }
            };

            var evaluated = rule.TryEvaluate(context, out var result, out var by);

            evaluated.Should().BeTrue();
            result.Should().BeTrue();
            by.Should().Be("GlobalDefault");
        }
    }
}
