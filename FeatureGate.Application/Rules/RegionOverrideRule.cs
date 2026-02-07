using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Rules;
using FeatureGate.Domain.Enums;

namespace FeatureGate.Application.Rules
{
    public class RegionOverrideRule : IFeatureRule
    {
        public bool CanApply(FeatureEvaluationContext context)
            => !string.IsNullOrEmpty(context.Region);

        public bool TryEvaluate(
            FeatureEvaluationContext context,
            out bool result,
            out string evaluatedBy)
        {
            var match = context.Overrides.FirstOrDefault(o =>
                o.TargetType == OverrideType.Region &&
                o.TargetId == context.Region);

            if (match is null)
            {
                result = default;
                evaluatedBy = string.Empty;
                return false;
            }

            result = match.State;
            evaluatedBy = "RegionOverride";
            return true;
        }
    }
}
