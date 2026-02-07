using FeatureGate.Domain.Entities;

namespace FeatureGate.Application.Interfaces.Repositories
{
    public interface IFeatureRepository: IGenericRepository<Feature>
    {
        Task<Feature?> GetByKeyAsync(string featureKey);
    }
}
