using AutoMapper;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Common;
using FeatureGate.Domain.Entities;

namespace FeatureGate.Application.Services.Common
{
    public abstract class CrudService<TEntity, TDto>
    : ICrudService<TDto>
    where TEntity : CommonEntity
    {
        protected readonly IGenericRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        protected CrudService(
            IGenericRepository<TEntity> repository,
            IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            return entity == null ? default : Mapper.Map<TDto>(entity);
        }

        public virtual async Task<IList<TDto>> GetAllAsync()
        {
            var entities = await Repository.GetAllAsync();
            return Mapper.Map<IList<TDto>>(entities);
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            var entity = Mapper.Map<TEntity>(dto);
            entity.Id = Guid.NewGuid();
            await Repository.AddAsync(entity);
            await Repository.SaveChangesAsync();

            return Mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> UpdateAsync(Guid id, TDto dto)
        {
            var existing = await Repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Entity not found");

            // Map updated values onto existing entity
            Mapper.Map(dto, existing);

            Repository.Update(existing);
            await Repository.SaveChangesAsync();

            return Mapper.Map<TDto>(existing);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Entity not found");

            Repository.Remove(entity);
            await Repository.SaveChangesAsync();
        }
    }
}
