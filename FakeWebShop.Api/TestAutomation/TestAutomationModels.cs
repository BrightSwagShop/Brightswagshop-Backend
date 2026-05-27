namespace FakeWebShop.Api.TestAutomation;

public enum TestAutomationSuite
{
    Api,
    Frontend,
    E2e
}

public enum TestAutomationRunStatus
{
    Queued,
    Running,
    Succeeded,
    Failed
}

public sealed record CreateTestAutomationRunRequest(TestAutomationSuite Suite);

public sealed record TestAutomationRunDto(
    Guid Id,
    TestAutomationSuite Suite,
    TestAutomationRunStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    int? ExitCode,
    string ReportPath,
    IReadOnlyCollection<string> OutputTail,
    string? ErrorMessage);
