using FeatureGate.Application.Evaluation;

namespace FeatureGate.Application.Interfaces.Rules
{
    public interface IFeatureRule
    {
        bool CanApply(FeatureEvaluationContext context);
        bool TryEvaluate(
            FeatureEvaluationContext context,
            out bool result,
            out string evaluatedBy);
    }
}
