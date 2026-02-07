using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.DTOs.Evaluations
{
    [ExcludeFromCodeCoverage]
    public record FeatureEvaluationRequestDto
    {
        public string FeatureKey { get; set; } = null!;
        public string? UserId { get; set; }
        public string? GroupId { get; set; }
        public string? Region { get; set; }
    }
}
