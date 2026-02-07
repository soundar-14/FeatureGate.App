using FeatureGate.Application.DTOs.Evaluations;
using FeatureGate.Application.Interfaces.Services.Features;
using Microsoft.AspNetCore.Mvc;

namespace FeatureGate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureEvaluationController : ControllerBase
    {
        private readonly IFeatureEvaluationService _evaluationService;

        public FeatureEvaluationController(
            IFeatureEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [HttpPost]
        public async Task<IActionResult> Evaluate(
            FeatureEvaluationRequestDto request)
        {
            var result = await _evaluationService.EvaluateAsync(request);
            return Ok(result);
        }
    }
}
