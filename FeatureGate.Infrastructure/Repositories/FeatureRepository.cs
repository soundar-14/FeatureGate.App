using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Domain.Entities;
using FeatureGate.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FeatureGate.Infrastructure.Repositories
{
    public class FeatureRepository : GenericRepository<Feature>, IFeatureRepository
    {
        private readonly FeatureGateDbContext _db;

        public FeatureRepository(FeatureGateDbContext db): base(db)
        {
            _db = db;
        }

        public async Task<Feature?> GetByKeyAsync(string featureKey)
        {
            return await _db.Features
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Key == featureKey);
        }
    }
}
