using AutoMapper;
using FeatureGate.Application.DTOs.Features;
using FeatureGate.Application.Helpers.Constants;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Interfaces.Services.Features;
using FeatureGate.Application.Services.Common;
using FeatureGate.Domain.Entities;

namespace FeatureGate.Application.Services.Features
{
    public class FeatureService
     : CrudService<Feature, FeatureDto>, IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;
        private readonly ICacheService _cacheService;

        public FeatureService(
            IFeatureRepository featureRepository,
            IMapper mapper,
            ICacheService cacheService)
            : base(featureRepository, mapper)
        {
            _featureRepository = featureRepository;
            _cacheService = cacheService;
        }

        public override async Task<FeatureDto> CreateAsync(FeatureDto dto)
        {
            dto.Key = dto.Key.ToLowerInvariant();

            var exists = await _featureRepository.GetByKeyAsync(dto.Key);
            if (exists != null)
                throw new InvalidOperationException("Feature key already exists");

            return await base.CreateAsync(dto);
        }

        public override async Task<FeatureDto> UpdateAsync(Guid id, FeatureDto dto)
        {
            dto.Key = dto.Key.ToLowerInvariant();

            var exists = await _featureRepository.GetByKeyAsync(dto.Key);
            if (exists != null && exists.Id != id)
                throw new InvalidOperationException("Feature key already exists");


            var result = await base.UpdateAsync(id, dto);

            // Clear all cached evaluations for this feature
            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.FeaturePrefix(dto.Key));

            return result;
        }

        public override async Task DeleteAsync(Guid id)
        {
            var feature = await Repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Feature not found");

            await base.DeleteAsync(id);

            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.FeaturePrefix(feature.Key));
        }
    }
}
