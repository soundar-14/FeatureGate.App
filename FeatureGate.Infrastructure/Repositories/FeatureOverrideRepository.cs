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

        public async Task<bool> UserExistsAsync(Guid featureId, string userId)
        {
            return await _db.FeatureOverrides.AnyAsync(f => f.FeatureId == featureId && f.TargetType == Domain.Enums.OverrideType.User &&  f.TargetId == userId);
        }

        public async Task<bool> GroupExistsAsync(Guid featureId, string groupId)
        {
            return await _db.FeatureOverrides.AnyAsync(f => f.FeatureId == featureId && f.TargetType == Domain.Enums.OverrideType.Group && f.TargetId == groupId);
        }

        public async Task<bool> RegionExistsAsync(Guid featureId, string regionCode)
        {
            return await _db.FeatureOverrides.AnyAsync(f => f.FeatureId == featureId && f.TargetType == Domain.Enums.OverrideType.Region && f.TargetId == regionCode);
        }
    }
}
