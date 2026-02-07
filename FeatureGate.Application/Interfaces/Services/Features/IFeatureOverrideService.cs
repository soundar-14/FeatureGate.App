using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Interfaces.Services.Common;

namespace FeatureGate.Application.Interfaces.Services.Features
{
    public interface IFeatureOverrideService
    : ICrudService<FeatureOverrideDto>
    {
        // Future extension:
        // Task<IList<FeatureOverrideDto>> GetByFeatureIdAsync(Guid featureId);
    }
}
