using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

[Binding]
public class ProductsStepDefinitions
{
    private readonly TestScenarioContext _ctx;
    private readonly RestClient _adminClient;
    private RestClient? _authClient;

    public ProductsStepDefinitions(ScenarioContext scenarioContext)
    {
        _ctx = scenarioContext.Get<TestScenarioContext>();
        _adminClient = ApiContext.CreateClient("admin");
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_ctx.CreatedProductId != null)
        {
            var cleanup = new RestRequest($"/api/products/{_ctx.CreatedProductId}", Method.Delete);
            await _adminClient.ExecuteAsync(cleanup);
            _ctx.CreatedProductId = null;
        }

        _authClient?.Dispose();
        _adminClient.Dispose();
    }

    // -----------------------------------------------------------------------
    // Given
    // -----------------------------------------------------------------------

    [Given("I have a valid mug payload")]
    public void GivenIHaveAValidMugPayload()
    {
        // Stored so other steps can reference it; actual body built inline
        _ctx.Payload = CreateMugBody();
    }

    [Given("I am authenticated as a regular user for products API")]
    public void GivenIAmAuthenticatedAsRegularUserForProducts()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("user");
    }

    [Given("I am authenticated as an admin user for products API")]
    public void GivenIAmAuthenticatedAsAdminUserForProducts()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("admin");
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When(@"I GET ""([^""]*)""")]
    public async Task WhenIGet(string path)
    {
        var request = new RestRequest(path, Method.Get);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When(@"^I POST ""/api/products"" with the payload$")]
    public async Task WhenIPostProductsWithPayload()
    {
        var request = new RestRequest("/api/products", Method.Post);
        request.AddStringBody(CreateMugJson(), DataFormat.Json);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When(@"^I POST ""/api/products"" as a regular user with the payload$")]
    public async Task WhenIPostProductsAsRegularUserWithPayload()
    {
        var client = _authClient ?? ApiContext.CreateClient("user");
        var request = new RestRequest("/api/products", Method.Post);
        request.AddStringBody(CreateMugJson(), DataFormat.Json);
        await StoreResponseAsync(await client.ExecuteAsync(request));
    }

    [When(@"I GET the created product by id")]
    public async Task WhenIGetCreatedProductById()
    {
        var request = new RestRequest($"/api/products/{_ctx.CreatedProductId}", Method.Get);
        await StoreResponseAsync(await _adminClient.ExecuteAsync(request));
    }

    [When(@"I DELETE the created product by id")]
    public async Task WhenIDeleteCreatedProductById()
    {
        var request = new RestRequest($"/api/products/{_ctx.CreatedProductId}", Method.Delete);
        var response = await _adminClient.ExecuteAsync(request);
        await StoreResponseAsync(response);
        if (response.StatusCode == HttpStatusCode.NoContent)
            _ctx.CreatedProductId = null;
    }

    [When(@"I DELETE ""([^""]*)"" as a regular user")]
    public async Task WhenIDeleteAsRegularUser(string path)
    {
        var client = _authClient ?? ApiContext.CreateClient("user");
        var request = new RestRequest(path, Method.Delete);
        await StoreResponseAsync(await client.ExecuteAsync(request));
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then(@"the response status should be (\d+)")]
    public void ThenResponseStatusShouldBe(int statusCode)
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(statusCode);
    }

    [Then("the response should be an array")]
    public void ThenResponseShouldBeArray()
    {
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!.Type.Should().Be(JTokenType.Array);
    }

    [Then("I store the created product id")]
    public void ThenIStoreCreatedProductId()
    {
        _ctx.LastBody.Should().NotBeNull();
        var id = _ctx.LastBody!["id"]?.ToString();
        id.Should().NotBeNullOrEmpty();
        _ctx.CreatedProductId = id;
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

    private static string CreateMugJson()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return $@"{{
  ""$type"": ""SimpleProduct"",
  ""name"": ""Playwright Mug {unique}"",
  ""description"": ""Simple API test product"",
  ""price"": 9.99,
  ""category"": ""Drinkartikelen"",
  ""productType"": ""Mok"",
  ""isActive"": true,
  ""kleuren"": [
    {{
      ""kleur"": ""Zwart"",
      ""imageUrl"": ""https://example.com/mug-black.png"",
      ""stock"": 10,
      ""sku"": ""MUG-{unique}""
    }}
  ]
}}";
    }

    private static object CreateMugBody() => new { type = "SimpleProduct" };
}
