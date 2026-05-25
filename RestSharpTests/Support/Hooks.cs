using Reqnroll;

namespace RestSharpTests.Support;

/// <summary>
/// Reqnroll Before/After hooks that inject shared state into scenarios.
/// </summary>
[Binding]
public class Hooks
{
    private readonly ScenarioContext _scenarioContext;

    public Hooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _scenarioContext.Set(new TestScenarioContext());
    }
}
