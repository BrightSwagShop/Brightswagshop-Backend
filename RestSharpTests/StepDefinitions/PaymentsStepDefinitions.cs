using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

[Binding]
public class PaymentsStepDefinitions
{
    private readonly TestScenarioContext _ctx;
    private readonly RestClient _client;

    public PaymentsStepDefinitions(ScenarioContext scenarioContext)
    {
        _ctx = scenarioContext.Get<TestScenarioContext>();
        _client = ApiContext.CreateClient("anonymous");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _client.Dispose();
    }

    // -----------------------------------------------------------------------
    // Given
    // -----------------------------------------------------------------------

    [Given("I create an empty order for payments")]
    public async Task GivenICreateAnEmptyOrderForPayments()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var payload = new
        {
            userId = $"order-user-{unique}",
            items = Array.Empty<object>()
        };

        var request = new RestRequest("/api/orders", Method.Post);
        request.AddJsonBody(payload);
        var response = await _client.ExecuteAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created,
            because: "Empty order creation must succeed (201)");

        var body = JToken.Parse(response.Content!);
        body["id"].Should().NotBeNull();
        _ctx.CreatedOrderId = body["id"]!.ToString();
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When("I create checkout session for unknown order id")]
    public async Task WhenICreateCheckoutSessionForUnknownOrderId()
    {
        var request = new RestRequest("/api/payments/000000000000000000000000/checkout", Method.Post);
        await StoreResponseAsync(await _client.ExecuteAsync(request));
    }

    [When("I create checkout session for the stored payments order")]
    public async Task WhenICreateCheckoutSessionForStoredOrder()
    {
        var request = new RestRequest($"/api/payments/{_ctx.CreatedOrderId}/checkout", Method.Post);
        await StoreResponseAsync(await _client.ExecuteAsync(request));
    }

    [When("I POST Stripe webhook event without signature")]
    public async Task WhenIPostStripeWebhookWithoutSignature()
    {
        var request = new RestRequest("/api/webhooks/stripe", Method.Post);
        request.AddJsonBody(new { id = "evt_test", type = "checkout.session.completed" });
        await StoreResponseAsync(await _client.ExecuteAsync(request));
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then(@"the payments response status should be (\d+)")]
    public void ThenPaymentsResponseStatusShouldBe(int statusCode)
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(statusCode);
    }

    [Then(@"the payments response should contain text ""([^""]*)""")]
    public void ThenPaymentsResponseShouldContainText(string text)
    {
        _ctx.LastBodyText.Should().NotBeNull();
        _ctx.LastBodyText!.Should().Contain(text);
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
