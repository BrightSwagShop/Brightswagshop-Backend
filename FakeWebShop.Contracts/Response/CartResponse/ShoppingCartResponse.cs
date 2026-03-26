using System;

namespace FakeWebShop.Contracts.Response.CartResponse;

public class ShoppingCartResponse
{
    public string Id { get; set; } = null!;
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public List<CartItemResponse> Items { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
    public decimal TotalPrice { get; set; }
}
