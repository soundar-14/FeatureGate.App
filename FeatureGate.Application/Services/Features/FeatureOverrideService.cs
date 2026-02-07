using AutoMapper;
using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Helpers.Constants;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Interfaces.Services.Features;
using FeatureGate.Application.Services.Common;
using FeatureGate.Domain.Entities;
using FeatureGate.Domain.Enums;

namespace FeatureGate.Application.Services.Features
{
    public class FeatureOverrideService
    : CrudService<FeatureOverride, FeatureOverrideDto>, IFeatureOverrideService
    {
        private readonly ICacheService _cacheService;
        private readonly IFeatureOverrideRepository _repository;
        public FeatureOverrideService(
            IFeatureOverrideRepository repository,
            ICacheService cacheService,
            IMapper mapper)
            : base(repository, mapper)
        {
            _cacheService = cacheService;
            _repository = repository;

        }

        public override async Task<FeatureOverrideDto> CreateAsync(
            FeatureOverrideDto dto)
        {
            Validate(dto);

            var exists = dto.TargetType switch
            {
                OverrideType.User =>
                    await _repository.UserExistsAsync(dto.FeatureId, dto.TargetId),

                OverrideType.Group =>
                    await _repository.GroupExistsAsync(dto.FeatureId, dto.TargetId),

                OverrideType.Region =>
                    await _repository.RegionExistsAsync(dto.FeatureId, dto.TargetId),

                _ => false
            };

            if(exists)
                throw new ArgumentException($"TargetId '{dto.TargetId}' already exist for Feature '{dto.FeatureId}' and TargetType '{dto.TargetType}'");

            var result = await base.CreateAsync(dto);

            // Overrides affect evaluation → clear all feature caches
            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.FeatureAllPrefix);

            return result;
        }

        public override async Task<FeatureOverrideDto> UpdateAsync(
            Guid id,
            FeatureOverrideDto dto)
        {
            Validate(dto);
            var result = await base.UpdateAsync(id, dto);

            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.FeatureAllPrefix);

            return result;
        }

        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);

            await _cacheService.RemoveByPrefixAsync(
                CacheKeys.FeatureAllPrefix);
        }


        private static void Validate(FeatureOverrideDto dto)
        {
            if (dto.FeatureId == Guid.Empty)
                throw new ArgumentException("FeatureId is required");

            if (string.IsNullOrWhiteSpace(dto.TargetId))
                throw new ArgumentException("TargetId is required");
        }
    }
}
