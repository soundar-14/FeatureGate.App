using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Domain.Entities;
using FeatureGate.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FeatureGate.Infrastructure.Repositories
{
    public class FeatureOverrideRepository : GenericRepository<FeatureOverride>, IFeatureOverrideRepository
    {
        private readonly FeatureGateDbContext _db;

        public FeatureOverrideRepository(FeatureGateDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IList<FeatureOverride>>
            GetByFeatureIdAsync(Guid featureId)
        {
            return await _db.FeatureOverrides
                .AsNoTracking()
                .Where(o => o.FeatureId == featureId)
                .ToListAsync();
        }
    }
}
