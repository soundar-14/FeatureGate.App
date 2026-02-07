using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Rules;

namespace FeatureGate.Application.Rules
{
    public class DefaultRule : IFeatureRule
    {
        public bool CanApply(FeatureEvaluationContext context) => true;

        public bool TryEvaluate(
            FeatureEvaluationContext context,
            out bool result,
            out string evaluatedBy)
        {
            result = context.Feature.DefaultState;
            evaluatedBy = "GlobalDefault";
            return true;
        }
    }
}
