using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugSettingsController(IDebugStateService service) : ControllerBase
{
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var state = await service.GetAllStatesAsync();
        return Ok(state);
    }

    [HttpPost("toggle/{feature}")]
    public async Task<IActionResult> ToggleFeature(string feature)
    {
        var currentValue = await service.GetStateAsync(feature);
        await service.SetStateAsync(feature, !currentValue);
        return Ok(new { feature, enabled = !currentValue });
    }
}
