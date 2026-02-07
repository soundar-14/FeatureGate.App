using FeatureGate.Application.DTOs.Features;
using FeatureGate.Application.Interfaces.Services.Features;
using Microsoft.AspNetCore.Mvc;

namespace FeatureGate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var feature = await _featureService.GetByIdAsync(id);
            return feature == null ? NotFound() : Ok(feature);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _featureService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeatureDto dto)
            => Ok(await _featureService.CreateAsync(dto));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FeatureDto dto)
            => Ok(await _featureService.UpdateAsync(id, dto));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _featureService.DeleteAsync(id);
            return NoContent();
        }
    }
}
