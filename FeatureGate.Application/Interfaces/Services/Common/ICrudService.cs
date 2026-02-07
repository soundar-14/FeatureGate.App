namespace FeatureGate.Application.Interfaces.Services.Common
{
    public interface ICrudService<TDto>
    {
        Task<TDto?> GetByIdAsync(Guid id);
        Task<IList<TDto>> GetAllAsync();

        Task<TDto> CreateAsync(TDto dto);
        Task<TDto> UpdateAsync(Guid id, TDto dto);
        Task DeleteAsync(Guid id);
    }
}
