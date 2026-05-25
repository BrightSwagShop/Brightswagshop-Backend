using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;
using RestSharp;
using RestSharpTests.Support;

namespace RestSharpTests.StepDefinitions;

[Binding]
public class UsersStepDefinitions
{
    private readonly TestScenarioContext _ctx;
    private readonly RestClient _client;

    public UsersStepDefinitions(ScenarioContext scenarioContext)
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

    [Given("I prepare a unique user registration payload")]
    public void GivenIPrepareUniqueUserRegistrationPayload()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ctx.UserUsername = $"api-user-{unique}";
        _ctx.UserPassword = $"P@ss-{unique}";
    }

    [Given("I register a unique public user")]
    public async Task GivenIRegisterUniquePublicUser()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ctx.UserUsername = $"api-user-{unique}";
        _ctx.UserPassword = $"P@ss-{unique}";

        var request = new RestRequest("/api/users/register", Method.Post);
        request.AddJsonBody(new { username = _ctx.UserUsername, password = _ctx.UserPassword });
        var response = await _client.ExecuteAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var body = JToken.Parse(response.Content!);
        body["id"].Should().NotBeNull();
    }

    [Given("I prepare credentials for an unknown public user")]
    public void GivenIPrepareCredentialsForUnknownPublicUser()
    {
        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ctx.UserUsername = $"unknown-user-{unique}";
        _ctx.UserPassword = $"Wrong-{unique}";
    }

    // -----------------------------------------------------------------------
    // When
    // -----------------------------------------------------------------------

    [When(@"^I POST ""/api/users/register"" with the user payload$")]
    public async Task WhenIPostUsersRegister()
    {
        var request = new RestRequest("/api/users/register", Method.Post);
        request.AddJsonBody(new { username = _ctx.UserUsername, password = _ctx.UserPassword });
        await StoreUsersResponseAsync(await _client.ExecuteAsync(request));
    }

    [When(@"^I POST ""/api/users/login"" with the user payload$")]
    public async Task WhenIPostUsersLoginWithPayload()
    {
        var request = new RestRequest("/api/users/login", Method.Post);
        request.AddJsonBody(new { username = _ctx.UserUsername, password = _ctx.UserPassword });
        await StoreUsersResponseAsync(await _client.ExecuteAsync(request));
    }

    [When(@"^I POST ""/api/users/login"" with the same user credentials$")]
    public async Task WhenIPostUsersLoginWithSameCredentials()
    {
        var request = new RestRequest("/api/users/login", Method.Post);
        request.AddJsonBody(new { username = _ctx.UserUsername, password = _ctx.UserPassword });
        await StoreUsersResponseAsync(await _client.ExecuteAsync(request));
    }

    // -----------------------------------------------------------------------
    // Then
    // -----------------------------------------------------------------------

    [Then(@"the users response status should be (\d+)")]
    public void ThenUsersResponseStatusShouldBe(int statusCode)
    {
        _ctx.LastResponse.Should().NotBeNull();
        ((int)_ctx.LastResponse!.StatusCode).Should().Be(statusCode);
    }

    [Then("the users response should contain a user id")]
    public void ThenUsersResponseShouldContainUserId()
    {
        _ctx.LastBody.Should().NotBeNull();
        _ctx.LastBody!["id"].Should().NotBeNull();
        _ctx.LastBody["id"]!.ToString().Should().NotBeNullOrEmpty();
    }

    [Then("the users response username should match the request")]
    public void ThenUsersResponseUsernameShouldMatch()
    {
        _ctx.LastBody.Should().NotBeNull();
        // Response may have username directly or nested under 'user'
        var responseUsername = _ctx.LastBody!["username"]?.ToString()
            ?? _ctx.LastBody["user"]?["username"]?.ToString();
        responseUsername.Should().Be(_ctx.UserUsername);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private async Task StoreUsersResponseAsync(RestResponse response)
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
