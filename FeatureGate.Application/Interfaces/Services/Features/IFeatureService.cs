using FeatureGate.Application.DTOs.Features;
using FeatureGate.Application.Interfaces.Services.Common;

namespace FeatureGate.Application.Interfaces.Services.Features
{
    public interface IFeatureService : ICrudService<FeatureDto>
    {
        // Extension point for future feature-specific operations
        // e.g. Task<FeatureDto?> GetByKeyAsync(string key);
    }
}
