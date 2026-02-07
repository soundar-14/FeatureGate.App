using FeatureGate.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class FeatureOverride : CommonEntity
    {
        public Guid FeatureId { get; set; }

        public OverrideType TargetType { get; set; } // User, Group, Region
        public string TargetId { get; set; } = null!;
        public bool State { get; set; }

        public Feature Feature { get; set; } = null!;
    }
}
