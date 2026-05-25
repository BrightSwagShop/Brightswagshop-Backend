using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

/// <summary>
/// Step definitions for Shopping Cart API feature.
///
/// Uses a static BeforeAllTests hook to seed a product via admin context before any
/// scenario runs, and cleans up all created carts + the seeded product in AfterAllTests.
/// </summary>
[Binding]
public class ShoppingCartStepDefinitions
{
    // Shared state across all Shopping Cart scenarios (seeded once per test run)
    private static string? _seededProductId;
    private static readonly HashSet<string> CreatedCartIds = new();
    private static readonly long Unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private static readonly string SmokeUserId = $"pw-user-{Unique}";
    private static readonly string DeleteUserId = $"pw-user-{Unique}-delete";

    private static RestClient? _adminClientStatic;
    private static RestClient? _anonClientStatic;

    private readonly TestScenarioContext _ctx;

    public ShoppingCartStepDefinitions(ScenarioContext scenarioContext)
    {
        _ctx = scenarioContext.Get<TestScenarioContext>();
    }

    // -----------------------------------------------------------------------
    // BeforeAll / AfterAll — run once for the Shopping Cart feature
    // -----------------------------------------------------------------------

    [BeforeTestRun]
    public static async Task BeforeAllTests()
    {
        _adminClientStatic = ApiContext.CreateClient("admin");
        _anonClientStatic = ApiContext.CreateClient("anonymous");

        // Seed a product
        var request = new RestRequest("/api/products", Method.Post);
        request.AddStringBody(CreateMugJson(), DataFormat.Json);
        var response = await _adminClientStatic.ExecuteAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created,
            because: "BeforeAll product seeding must succeed");

        var body = JToken.Parse(response.Content!);
        _seededProductId = body["id"]!.ToString();
        _seededProductId.Should().NotBeNullOrEmpty();
    }

    [AfterTestRun]
    public static async Task AfterAllTests()
    {
        // Clean up all carts created during the run
        if (_anonClientStatic != null)
        {
            foreach (var cartId in CreatedCartIds.ToList())
            {
                var req = new RestRequest($"/api/shoppingcarts/{cartId}", Method.Delete);
                await _anonClientStatic.ExecuteAsync(req);
            }
        }

        // Clean up the seeded product
        if (_seededProductId != null && _adminClientStatic != null)
        {
            var req = new RestRequest($"/api/products/{_seededProductId}", Method.Delete);
            await _adminClientStatic.ExecuteAsync(req);
        }

        _adminClientStatic?.Dispose();
        _anonClientStatic?.Dispose();
    }

    // -----------------------------------------------------------------------
    // Given
    // -----------------------------------------------------------------------

    [Given(@"I prepare a shopping cart request for the smoke user with quantity (\d+)")]
    public void GivenIPrepareCartRequestForSmokeUser(int quantity)
    {
        _ctx.CartRequest = new
        {
            userId = SmokeUserId,
            items = new[] { new { productId = _seededProductId, quantity } }
        };
    }

    [Given(@"I prepare a shopping cart request for the delete user with quantity (\d+)")]
    public void GivenIPrepareCartRequestForDeleteUser(int quantity)
    {
        _ctx.CartRequest = new
        {
            userId = DeleteUserId,
            items = new[] { new { productId = _seededProductId, quantity } }
        };
    }

    [Given("I create and remember a shopping cart")]
    public async Task GivenICreateAndRememberACart()
    {
        await CreateCartAsync();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(201);
        var id = _ctx.LastBody!["id"]!.ToString();
        _ctx.RememberedCartId = id;
        CreatedCartIds.Add(id);
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When("I create the shopping cart")]
    public async Task WhenICreateTheShoppingCart()
    {
        await CreateCartAsync();
    }

    [When("I get the shopping cart for the smoke user")]
    public async Task WhenIGetCartForSmokeUser()
    {
        var request = new RestRequest($"/api/shoppingcarts/user/{SmokeUserId}", Method.Get);
        await StoreResponseAsync(await _anonClientStatic!.ExecuteAsync(request));
    }

    [When("I get the shopping cart for the delete user")]
    public async Task WhenIGetCartForDeleteUser()
    {
        var request = new RestRequest($"/api/shoppingcarts/user/{DeleteUserId}", Method.Get);
        await StoreResponseAsync(await _anonClientStatic!.ExecuteAsync(request));
    }

    [When("I delete the remembered shopping cart")]
    public async Task WhenIDeleteRememberedCart()
    {
        var request = new RestRequest($"/api/shoppingcarts/{_ctx.RememberedCartId}", Method.Delete);
        var response = await _anonClientStatic!.ExecuteAsync(request);
        await StoreResponseAsync(response);
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            CreatedCartIds.Remove(_ctx.RememberedCartId!);
            _ctx.RememberedCartId = null;
        }
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then(@"the shopping cart response status should be (\d+)")]
    public void ThenShoppingCartResponseStatusShouldBe(int statusCode)
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(statusCode);
    }

    [Then("the created shopping cart should match the smoke user")]
    public void ThenCreatedCartShouldMatchSmokeUser()
    {
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!["userId"]!.ToString().Should().Be(SmokeUserId);
        // sessionId should be null or absent for userId-based carts
        var sessionId = _ctx.LastBody["sessionId"];
        if (sessionId != null && sessionId.Type != JTokenType.Null)
        {
            sessionId.ToString().Should().BeNullOrEmpty();
        }
        _ctx.LastBody["id"]!.ToString().Should().NotBeNullOrEmpty();
        // totalPrice can be integer 0 or a float
        _ctx.LastBody["totalPrice"].Should().NotBeNull();
        _ctx.LastBody["totalPrice"]!.Type.Should().BeOneOf(JTokenType.Float, JTokenType.Integer);
        _ctx.LastBody["items"]!.Type.Should().Be(JTokenType.Array);
    }

    [Then(@"the created shopping cart should contain one item for seeded product with quantity (\d+)")]
    public void ThenCreatedCartShouldContainOneItemForSeededProduct(int quantity)
    {
        var items = _ctx.LastBody!["items"] as JArray;
        items.Should().NotBeNull();
        items!.Count.Should().Be(1);
        var first = items[0];
        first["productId"]!.ToString().Should().Be(_seededProductId);
        first["productName"]!.Type.Should().Be(JTokenType.String);
        first["unitPrice"]!.Type.Should().BeOneOf(JTokenType.Float, JTokenType.Integer);
        ((int)first["quantity"]!).Should().Be(quantity);
    }

    [Then("I remember the created shopping cart id")]
    public void ThenIRememberCreatedCartId()
    {
        var id = _ctx.LastBody!["id"]!.ToString();
        _ctx.RememberedCartId = id;
        CreatedCartIds.Add(id);
    }

    [Then("the returned shopping cart should belong to the smoke user")]
    public void ThenReturnedCartShouldBelongToSmokeUser()
    {
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!["userId"]!.ToString().Should().Be(SmokeUserId);
        _ctx.LastBody["id"]!.ToString().Should().NotBeNullOrEmpty();
        _ctx.LastBody["items"]!.Type.Should().Be(JTokenType.Array);
        (_ctx.LastBody["items"] as JArray)!.Count.Should().BeGreaterThan(0);
    }

    [Then("the returned shopping cart should contain the seeded product")]
    public void ThenReturnedCartShouldContainSeededProduct()
    {
        var items = _ctx.LastBody!["items"] as JArray;
        items.Should().NotBeNull();
        items![0]["productId"]!.ToString().Should().Be(_seededProductId);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private async Task CreateCartAsync()
    {
        var request = new RestRequest("/api/shoppingcarts", Method.Post);
        request.AddJsonBody(_ctx.CartRequest!);
        await StoreResponseAsync(await _anonClientStatic!.ExecuteAsync(request));
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
  ""name"": ""Cart Seed Mug {unique}"",
  ""description"": ""Seeded for cart tests"",
  ""price"": 9.99,
  ""category"": ""Drinkartikelen"",
  ""productType"": ""Mok"",
  ""isActive"": true,
  ""kleuren"": [
    {{
      ""kleur"": ""Zwart"",
      ""imageUrl"": ""https://example.com/mug-black.png"",
      ""stock"": 10,
      ""sku"": ""CART-MUG-{unique}""
    }}
  ]
}}";
    }
}
