using AutoMapper;
using FeatureGate.Application.DTOs.Evaluations;
using FeatureGate.Application.Evaluation;
using FeatureGate.Application.Interfaces.Repositories;
using FeatureGate.Application.Interfaces.Services.Cache;
using FeatureGate.Application.Interfaces.Services.Features;
using FeatureGate.Application.Rules.Engines;
using FluentValidation;

namespace FeatureGate.Application.Services.Features
{
    public class FeatureEvaluationService : IFeatureEvaluationService
    {
        private readonly IFeatureRepository _featureRepo;
        private readonly IFeatureOverrideRepository _overrideRepo;
        private readonly FeatureRuleEngine _ruleEngine;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IValidator<FeatureEvaluationRequestDto> _validator;

        public FeatureEvaluationService(
            IFeatureRepository featureRepo,
            IFeatureOverrideRepository overrideRepo,
            FeatureRuleEngine ruleEngine,
            IValidator<FeatureEvaluationRequestDto> validator,
            IMapper mapper,
            ICacheService cacheService)
        {
            _featureRepo = featureRepo;
            _overrideRepo = overrideRepo;
            _ruleEngine = ruleEngine;
            _mapper = mapper;
            _cacheService = cacheService;
            _validator = validator;
        }

        public async Task<FeatureEvaluationResultDto> EvaluateAsync(
            FeatureEvaluationRequestDto request)
        {
            await _validator.ValidateAndThrowAsync(request);
            var featureKey = request.FeatureKey.ToLowerInvariant();

            var cacheKey =
                $"feature:{featureKey}:user:{request.UserId}:group:{request.GroupId}:region:{request.Region}";

            // 1️⃣ Try Redis cache
            var cached =
                await _cacheService.GetAsync<FeatureEvaluationResultDto>(cacheKey);

            if (cached != null)
                return cached;

            // 2️⃣ Load feature
            var feature = await _featureRepo.GetByKeyAsync(featureKey)
                ?? throw new KeyNotFoundException($"Feature '{featureKey}' not found");

            // 3️⃣ Load overrides
            var overrides =
                await _overrideRepo.GetByFeatureIdAsync(feature.Id);

            // 4️⃣ Build evaluation context
            var context = new FeatureEvaluationContext
            {
                Feature = feature,
                Overrides = overrides,
                UserId = request.UserId,
                GroupId = request.GroupId,
                Region = request.Region
            };

            // 5️⃣ Evaluate rules
            var evaluation = _ruleEngine.Evaluate(context);

            // 6️⃣ Map result
            var result = _mapper.Map<FeatureEvaluationResultDto>(evaluation);
            result.FeatureKey = featureKey;

            // 7️⃣ Store in Redis
            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(10));

            return result;
        }
    }
}
