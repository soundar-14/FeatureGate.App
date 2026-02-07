using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FeatureGate.Infrastructure.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        protected readonly FeatureGateDbContext _db;
        protected readonly DbSet<T> _set;

        public GenericRepository(FeatureGateDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
            => await _set.FindAsync(id);

        public async Task<IList<T>> GetAllAsync()
            => await _set.AsNoTracking().ToListAsync();

        public async Task AddAsync(T entity)
            => await _set.AddAsync(entity);

        public void Update(T entity)
            => _set.Update(entity);

        public void Remove(T entity)
            => _set.Remove(entity);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }

}
