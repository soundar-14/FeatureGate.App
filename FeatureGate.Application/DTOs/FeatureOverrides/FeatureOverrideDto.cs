using FeatureGate.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.DTOs.FeatureOverrides
{
    [ExcludeFromCodeCoverage]
    public record FeatureOverrideDto
    {
        public Guid? Id { get; set; }

        public Guid FeatureId { get; set; }
        public OverrideType TargetType { get; set; }
        public string TargetId { get; set; } = null!;
        public bool State { get; set; }
    }
}
