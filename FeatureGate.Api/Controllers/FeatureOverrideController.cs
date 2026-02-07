using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.Interfaces.Services.Features;
using Microsoft.AspNetCore.Mvc;

namespace FeatureGate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureOverrideController : ControllerBase
    {
        private readonly IFeatureOverrideService _overrideService;

        public FeatureOverrideController(
            IFeatureOverrideService overrideService)
        {
            _overrideService = overrideService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _overrideService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _overrideService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeatureOverrideDto dto)
            => Ok(await _overrideService.CreateAsync(dto));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FeatureOverrideDto dto)
            => Ok(await _overrideService.UpdateAsync(id, dto));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _overrideService.DeleteAsync(id);
            return NoContent();
        }
    }
}
