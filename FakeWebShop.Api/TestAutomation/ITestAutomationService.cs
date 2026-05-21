namespace FakeWebShop.Api.TestAutomation;

public interface ITestAutomationService
{
    Task<TestAutomationRunDto> StartAsync(TestAutomationSuite suite, string? apiBaseUrl = null, CancellationToken cancellationToken = default);

    Task<TestAutomationRunDto?> GetAsync(Guid runId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TestAutomationRunDto>> GetLatestAsync(CancellationToken cancellationToken = default);
}
