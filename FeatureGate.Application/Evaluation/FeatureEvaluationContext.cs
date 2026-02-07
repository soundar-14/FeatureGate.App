using FeatureGate.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.Evaluation
{
    [ExcludeFromCodeCoverage]
    public class FeatureEvaluationContext
    {
        public Feature Feature { get; init; } = null!;
        public IList<FeatureOverride> Overrides { get; init; } = [];
        public string? UserId { get; init; }
        public string? GroupId { get; init; }
        public string? Region { get; init; }
    }
}
