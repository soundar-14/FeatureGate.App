using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Feature : CommonEntity
    {
        public string Key { get; set; } = null!; // stored lowercase
        public bool DefaultState { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<FeatureOverride> Overrides { get; set; }
     = new List<FeatureOverride>();
    }
}
