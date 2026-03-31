using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeWebShop.Api;
using FakeWebShop.Contracts.Request.ApplyDiscountRequest;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace FakeWebShop.Api.Tests.Discount
{
    public class ShoppingCartControllerDiscountTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public ShoppingCartControllerDiscountTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ApplyDiscount_ReturnsConflict_WhenAlreadyApplied()
        {
            // Arrange: create cart, apply discount, then try again
            var cartRequest = new ShoppingCartRequest { UserId = "user1", SessionId = "sess1", Items = [] };
            var cartResp = await _client.PostAsync("/api/shoppingcarts", new StringContent(JsonConvert.SerializeObject(cartRequest), Encoding.UTF8, "application/json"));
            var cart = JsonConvert.DeserializeObject<ShoppingCartResponse>(await cartResp.Content.ReadAsStringAsync());
            var discountBody = new ApplyDiscountRequest { Code = "CODE10" };
            var applyResp1 = await _client.PostAsync($"/api/shoppingcarts/{cart.Id}/apply-discount", new StringContent(JsonConvert.SerializeObject(discountBody), Encoding.UTF8, "application/json"));
            var applyResp2 = await _client.PostAsync($"/api/shoppingcarts/{cart.Id}/apply-discount", new StringContent(JsonConvert.SerializeObject(discountBody), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.Conflict, applyResp2.StatusCode);
        }
    }
}
