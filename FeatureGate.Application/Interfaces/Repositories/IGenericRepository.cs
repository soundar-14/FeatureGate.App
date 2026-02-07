using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureGate.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IList<T>> GetAllAsync();

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        Task SaveChangesAsync();
    }
}
