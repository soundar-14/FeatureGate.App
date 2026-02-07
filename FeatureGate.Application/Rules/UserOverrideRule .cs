using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Rules;
using FeatureGate.Domain.Enums;

namespace FeatureGate.Application.Rules
{
    public class UserOverrideRule : IFeatureRule
    {
        public bool CanApply(FeatureEvaluationContext context)
            => !string.IsNullOrEmpty(context.UserId);

        public bool TryEvaluate(
            FeatureEvaluationContext context,
            out bool result,
            out string evaluatedBy)
        {
            var match = context.Overrides.FirstOrDefault(o =>
                o.TargetType == OverrideType.User &&
                o.TargetId == context.UserId);

            if (match is null)
            {
                result = default;
                evaluatedBy = string.Empty;
                return false;
            }

            result = match.State;
            evaluatedBy = "UserOverride";
            return true;
        }
    }
}
