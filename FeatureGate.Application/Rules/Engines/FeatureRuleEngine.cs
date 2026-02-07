using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Rules;

namespace FeatureGate.Application.Rules.Engines
{
    public class FeatureRuleEngine
    {
        private readonly IEnumerable<IFeatureRule> _rules;

        public FeatureRuleEngine(IEnumerable<IFeatureRule> rules)
        {
            _rules = rules;
        }

        public (bool Result, string EvaluatedBy) Evaluate(
            FeatureEvaluationContext context)
        {
            foreach (var rule in _rules)
            {
                if (!rule.CanApply(context)) continue;

                if (rule.TryEvaluate(context, out var result, out var by))
                    return (result, by);
            }

            throw new InvalidOperationException("No rule applied.");
        }
    }
}
