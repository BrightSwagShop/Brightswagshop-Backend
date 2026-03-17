using System;

namespace FakeWebShop.Contracts.Request.CartRequest;

public class CartItemRequest
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
}
