using FakeWebShop.Api.TestAutomation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/admin/test-automation")]
[Authorize(AuthenticationSchemes = "AzureAd", Roles = "App.Admin")]
public sealed class TestAutomationController : ControllerBase
{
    private readonly ITestAutomationService _testAutomationService;

    public TestAutomationController(ITestAutomationService testAutomationService)
    {
        _testAutomationService = testAutomationService;
    }

    [HttpGet("runs/latest")]
    public async Task<ActionResult<IReadOnlyCollection<TestAutomationRunDto>>> GetLatestRuns(CancellationToken cancellationToken)
    {
        var runs = await _testAutomationService.GetLatestAsync(cancellationToken);
        return Ok(runs);
    }

    [HttpGet("runs/{runId:guid}")]
    public async Task<ActionResult<TestAutomationRunDto>> GetRun(Guid runId, CancellationToken cancellationToken)
    {
        var run = await _testAutomationService.GetAsync(runId, cancellationToken);
        return run is null ? NotFound() : Ok(run);
    }

    [HttpPost("runs")]
    public async Task<ActionResult<TestAutomationRunDto>> StartRun([FromBody] CreateTestAutomationRunRequest request, CancellationToken cancellationToken)
    {
        var apiBaseUrl = $"{Request.Scheme}://{Request.Host}";
        var run = await _testAutomationService.StartAsync(request.Suite, apiBaseUrl, cancellationToken);
        return Accepted(run);
    }
}