using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.DTOs.Evaluations
{
    [ExcludeFromCodeCoverage]
    public record FeatureEvaluationResultDto
    {
        public string FeatureKey { get; set; } = null!;
        public bool IsEnabled { get; set; }
        public string EvaluatedBy { get; set; } = null!;
    }
}
