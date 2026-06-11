using System.Net.Http;

namespace BrightestSwagShop.Tests.Helpers;

public class ToggleHelper
{
    private readonly HttpClient _client;

    public ToggleHelper()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5076");
    }

    public async Task ToggleFeature(string feature)
    {
        var response = await _client.PostAsync(
                $"/api/debug/toggle/{feature}",
                null
            );

            response.EnsureSuccessStatusCode();
       
    }
}