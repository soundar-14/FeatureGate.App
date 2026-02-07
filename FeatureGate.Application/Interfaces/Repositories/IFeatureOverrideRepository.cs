using FeatureGate.Domain.Entities;

namespace FeatureGate.Application.Interfaces.Repositories
{
    public interface IFeatureOverrideRepository : IGenericRepository<FeatureOverride>
    {
        Task<IList<FeatureOverride>> GetByFeatureIdAsync(Guid featureId);
    }
}
