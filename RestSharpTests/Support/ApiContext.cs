using RestSharp;

namespace RestSharpTests.Support;

/// <summary>
/// Factory for creating RestSharp clients with the appropriate auth headers for the custom
/// header-based auth handler used in the BrightSwagShop API during tests.
/// </summary>
public static class ApiContext
{
    public const string BaseUrl = "http://127.0.0.1:5076";

    /// <summary>
    /// Creates a RestSharp client authenticated for the given role.
    /// </summary>
    /// <param name="role">"admin", "user", or "anonymous"</param>
    public static RestClient CreateClient(string role = "anonymous")
    {
        var options = new RestClientOptions(BaseUrl);
        var client = new RestClient(options);

        if (role == "admin")
        {
            client.AddDefaultHeader("X-User-Role", "Admin");
            client.AddDefaultHeader("X-User-Id", "test-admin-user");
        }
        else if (role == "user")
        {
            client.AddDefaultHeader("X-User-Role", "User");
            client.AddDefaultHeader("X-User-Id", "test-user-user");
        }
        // anonymous: no headers added

        return client;
    }
}
