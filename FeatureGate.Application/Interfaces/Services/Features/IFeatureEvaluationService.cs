using FeatureGate.Application.DTOs.Evaluations;

namespace FeatureGate.Application.Interfaces.Services.Features
{
    public interface IFeatureEvaluationService
    {
        Task<FeatureEvaluationResultDto> EvaluateAsync(
            FeatureEvaluationRequestDto request);
    }
}
