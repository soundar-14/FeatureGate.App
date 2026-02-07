using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.Helpers.Constants
{
    [ExcludeFromCodeCoverage]
    public static class CacheKeys
    {
        public static string FeaturePrefix(string featureKey)
            => $"feature:{featureKey.ToLowerInvariant()}";

        public const string FeatureAllPrefix = "feature:";
    }
}
