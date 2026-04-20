using System;

namespace FakeWebShop.Contracts.Request.CartRequest;

public class ShoppingCartRequest
{
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public List<CartItemRequest> Items { get; set; } = new();
}
