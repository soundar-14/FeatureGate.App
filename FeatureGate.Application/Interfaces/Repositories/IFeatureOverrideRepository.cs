using FeatureGate.Domain.Entities;
using System.Text.RegularExpressions;

namespace FeatureGate.Application.Interfaces.Repositories
{
    public interface IFeatureOverrideRepository : IGenericRepository<FeatureOverride>
    {
        Task<IList<FeatureOverride>> GetByFeatureIdAsync(Guid featureId);
        Task<bool> UserExistsAsync(Guid featureId, string userId);
        Task<bool> GroupExistsAsync(Guid featureId, string groupId);
        Task<bool> RegionExistsAsync(Guid featureId, string regionCode);
    }
}
