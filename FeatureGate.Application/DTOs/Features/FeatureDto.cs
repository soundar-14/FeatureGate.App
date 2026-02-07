using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.DTOs.Features
{
    [ExcludeFromCodeCoverage]
    public record FeatureDto
    {
        public Guid? Id { get; set; }     // null = create, not null = update
        public string Key { get; set; } = null!;
        public bool DefaultState { get; set; }
        public string? Description { get; set; }
    }
}
