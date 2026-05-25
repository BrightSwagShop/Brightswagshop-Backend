using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

[Binding]
public class BackendStepDefinitions
{
    private readonly TestScenarioContext _ctx;
    private readonly RestClient _adminClient;
    private RestClient? _authClient;

    public BackendStepDefinitions(ScenarioContext scenarioContext)
    {
        _ctx = scenarioContext.Get<TestScenarioContext>();
        _adminClient = ApiContext.CreateClient("admin");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _authClient?.Dispose();
        _adminClient.Dispose();
    }

    // -----------------------------------------------------------------------
    // Given
    // -----------------------------------------------------------------------

    [Given("I am authenticated as a regular user for backend API")]
    public void GivenIAmAuthenticatedAsRegularUserForBackend()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("user");
    }

    [Given("I am authenticated as an admin user for backend API")]
    public void GivenIAmAuthenticatedAsAdminUserForBackend()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("admin");
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When("I GET backend categories")]
    public async Task WhenIGetBackendCategories()
    {
        var request = new RestRequest("/api/categories", Method.Get);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When("I GET backend product types")]
    public async Task WhenIGetBackendProductTypes()
    {
        var request = new RestRequest("/api/producttypes", Method.Get);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When("I POST backend image upload without file")]
    public async Task WhenIPostBackendImageUploadWithoutFile()
    {
        // Admin POST to /api/images/upload without any file → expect 400
        var request = new RestRequest("/api/images/upload", Method.Post);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When("I POST backend image upload as a regular user without file")]
    public async Task WhenIPostBackendImageUploadAsRegularUserWithoutFile()
    {
        var client = _authClient ?? ApiContext.CreateClient("user");
        var request = new RestRequest("/api/images/upload", Method.Post);
        await StoreResponseAsync(await client.ExecuteAsync(request));
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then(@"the backend response status should be (\d+)")]
    public void ThenBackendResponseStatusShouldBe(int statusCode)
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(statusCode);
    }

    [Then("the backend response should be a non-empty array")]
    public void ThenBackendResponseShouldBeNonEmptyArray()
    {
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!.Type.Should().Be(JTokenType.Array);
        (_ctx.LastBody as JArray)!.Count.Should().BeGreaterThan(0);
    }

    [Then("the first backend item should contain number id and string name")]
    public void ThenFirstBackendItemShouldContainNumberIdAndStringName()
    {
        var arr = _ctx.LastBody as JArray;
        arr.Should().NotBeNull();
        var first = arr![0];
        first["id"].Should().NotBeNull();
        first["id"]!.Type.Should().BeOneOf(JTokenType.Integer, JTokenType.Float);
        first["name"].Should().NotBeNull();
        first["name"]!.Type.Should().Be(JTokenType.String);
    }

    [Then("the first backend item should contain string name and string slug")]
    public void ThenFirstBackendItemShouldContainStringNameAndStringSlug()
    {
        var arr = _ctx.LastBody as JArray;
        arr.Should().NotBeNull();
        var first = arr![0];
        first["name"].Should().NotBeNull();
        first["name"]!.Type.Should().Be(JTokenType.String);
        first["slug"].Should().NotBeNull();
        first["slug"]!.Type.Should().Be(JTokenType.String);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private async Task StoreResponseAsync(RestResponse response)
    {
        _ctx.LastResponse = response;
        _ctx.LastBodyText = response.Content;
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            try { _ctx.LastBody = JToken.Parse(response.Content); }
            catch { /* not JSON */ }
        }
        await Task.CompletedTask;
    }
}
