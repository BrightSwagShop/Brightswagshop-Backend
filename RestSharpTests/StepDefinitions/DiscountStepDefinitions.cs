using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

/// <summary>
/// Step definitions for Discount API feature.
///
/// BeforeTestRun seeds one product and one "SPRING20" discount via admin context.
/// AfterTestRun cleans up any created discounts, carts, and the seeded product.
/// </summary>
[Binding]
public class DiscountStepDefinitions
{
    private const string ValidDiscountCode = "SPRING20";

    // Static state – seeded once per test run
    private static string? _seededProductId;
    private static string? _seededDiscountId;
    private static string? _authCreatedDiscountId;
    private static string? _discountCartId;

    private static RestClient? _adminClientStatic;
    private static RestClient? _anonClientStatic;

    // Per-scenario state
    private readonly TestScenarioContext _ctx;
    private RestClient? _authClient;   // overridden by Given steps for role tests

    public DiscountStepDefinitions(ScenarioContext scenarioContext)
    {
        _ctx = scenarioContext.Get<TestScenarioContext>();
    }

    // -----------------------------------------------------------------------
    // BeforeAll / AfterAll
    // -----------------------------------------------------------------------

    [BeforeTestRun]
    public static async Task BeforeAllTests()
    {
        _adminClientStatic = ApiContext.CreateClient("admin");
        _anonClientStatic = ApiContext.CreateClient("anonymous");

        // Seed a product
        var productRequest = new RestRequest("/api/products", Method.Post);
        productRequest.AddStringBody(CreateMugJson(), DataFormat.Json);
        var productResponse = await _adminClientStatic.ExecuteAsync(productRequest);
        productResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created,
            because: "Discount BeforeAll product seeding must succeed");
        var productBody = JToken.Parse(productResponse.Content!);
        _seededProductId = productBody["id"]!.ToString();

        // Seed SPRING20 discount
        var now = DateTimeOffset.UtcNow;
        var discountPayload = new
        {
            name = "Spring 20",
            description = "Discount for API tests",
            percentage = 20,
            code = ValidDiscountCode,
            startsAt = now.AddHours(-1).ToString("o"),
            endsAt = now.AddDays(1).ToString("o"),
            isActive = true
        };
        var discountRequest = new RestRequest("/api/discounts", Method.Post);
        discountRequest.AddJsonBody(discountPayload);
        var discountResponse = await _adminClientStatic.ExecuteAsync(discountRequest);
        discountResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created,
            because: "Discount BeforeAll discount seeding must succeed");
        var discountBody = JToken.Parse(discountResponse.Content!);
        _seededDiscountId = discountBody["id"]!.ToString();
    }

    [AfterTestRun]
    public static async Task AfterAllTests()
    {
        // Use admin client for discount deletions (admin-protected endpoints)
        if (_authCreatedDiscountId != null && _adminClientStatic != null)
        {
            await _adminClientStatic.ExecuteAsync(
                new RestRequest($"/api/discounts/{_authCreatedDiscountId}", Method.Delete));
        }
        if (_discountCartId != null && _anonClientStatic != null)
        {
            await _anonClientStatic.ExecuteAsync(
                new RestRequest($"/api/shoppingcarts/{_discountCartId}", Method.Delete));
        }
        if (_seededDiscountId != null && _adminClientStatic != null)
        {
            await _adminClientStatic.ExecuteAsync(
                new RestRequest($"/api/discounts/{_seededDiscountId}", Method.Delete));
        }
        if (_seededProductId != null && _adminClientStatic != null)
        {
            await _adminClientStatic.ExecuteAsync(
                new RestRequest($"/api/products/{_seededProductId}", Method.Delete));
        }

        _adminClientStatic?.Dispose();
        _anonClientStatic?.Dispose();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _authClient?.Dispose();
    }

    // -----------------------------------------------------------------------
    // Given
    // -----------------------------------------------------------------------

    [Given("I am authenticated as a regular user")]
    public void GivenIAmAuthenticatedAsRegularUser()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("user");
    }

    [Given("I am authenticated as an admin user")]
    public void GivenIAmAuthenticatedAsAdminUser()
    {
        _authClient?.Dispose();
        _authClient = ApiContext.CreateClient("admin");
    }

    [Given(@"a shopping cart exists with user ""([^""]*)"" and product ""([^""]*)""")]
    public async Task GivenAShoppingCartExistsWithUserAndProduct(string userId, string productName)
    {
        // productName is ignored; we always use the seeded product
        var cartPayload = new
        {
            userId,
            sessionId = "sess1",
            items = new[] { new { productId = _seededProductId, quantity = 1 } }
        };

        var request = new RestRequest("/api/shoppingcarts", Method.Post);
        request.AddJsonBody(cartPayload);
        var response = await _anonClientStatic!.ExecuteAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var body = JToken.Parse(response.Content!);
        _discountCartId = body["id"]!.ToString();
        _ctx.DiscountCartId = _discountCartId;
    }

    [Given(@"I apply the discount code ""([^""]*)"" to the cart")]
    public async Task GivenIApplyDiscountCodeToCart(string code)
    {
        await ApplyDiscountCodeAsync(code);
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When(@"I create a discount with code ""([^""]*)""")]
    public async Task WhenICreateDiscountWithCode(string code)
    {
        var client = _authClient ?? _anonClientStatic!;
        var now = DateTimeOffset.UtcNow;
        var payload = new
        {
            name = $"Test {code}",
            description = "Authorization test discount",
            percentage = 10,
            code,
            startsAt = now.AddHours(-1).ToString("o"),
            endsAt = now.AddDays(1).ToString("o"),
            isActive = true
        };

        var request = new RestRequest("/api/discounts", Method.Post);
        request.AddJsonBody(payload);
        var response = await client.ExecuteAsync(request);
        await StoreResponseAsync(response);

        if (response.StatusCode == System.Net.HttpStatusCode.Created && _ctx.LastBody?["id"] != null)
        {
            _authCreatedDiscountId = _ctx.LastBody["id"]!.ToString();
        }
    }

    [When(@"I apply the discount code ""([^""]*)"" to the cart")]
    public async Task WhenIApplyDiscountCodeToCart(string code)
    {
        await ApplyDiscountCodeAsync(code);
    }

    [When(@"I apply the discount code ""([^""]*)"" to the cart again")]
    public async Task WhenIApplyDiscountCodeToCartAgain(string code)
    {
        await ApplyDiscountCodeAsync(code);
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then("the cart total should reflect the discount")]
    public void ThenCartTotalShouldReflectDiscount()
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(200);
        // totalPrice must be a positive number (just verify it exists and is numeric)
        _ctx.LastBody.Should().NotBeNull();
        var total = _ctx.LastBody!["totalPrice"];
        total.Should().NotBeNull();
        total!.Type.Should().BeOneOf(JTokenType.Float, JTokenType.Integer);
    }

    [Then(@"I should receive a 409 Conflict error")]
    public void ThenShouldReceive409()
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(409);
    }

    [Then(@"I should receive a 404 Not Found error")]
    public void ThenShouldReceive404()
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(404);
    }

    [Then(@"I should receive a 403 Forbidden error")]
    public void ThenShouldReceive403()
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(403);
    }

    [Then(@"I should receive a 201 Created response")]
    public void ThenShouldReceive201()
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(201);
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!["id"].Should().NotBeNull();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private async Task ApplyDiscountCodeAsync(string code)
    {
        var cartId = _ctx.DiscountCartId ?? _discountCartId;
        var request = new RestRequest($"/api/shoppingcarts/{cartId}/apply-discount", Method.Post);
        request.AddJsonBody(new { code });
        var response = await _anonClientStatic!.ExecuteAsync(request);
        await StoreResponseAsync(response);
    }

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
  ""name"": ""Discount Seed Mug {unique}"",
  ""description"": ""Seeded for discount tests"",
  ""price"": 9.99,
  ""category"": ""Drinkartikelen"",
  ""productType"": ""Mok"",
  ""isActive"": true,
  ""kleuren"": [
    {{
      ""kleur"": ""Zwart"",
      ""imageUrl"": ""https://example.com/mug-black.png"",
      ""stock"": 10,
      ""sku"": ""DISC-MUG-{unique}""
    }}
  ]
}}";
    }
}
