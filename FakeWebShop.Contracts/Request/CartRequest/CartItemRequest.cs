using System;

namespace FakeWebShop.Contracts.Request.CartRequest;

public class CartItemRequest
{
    public string ProductId { get; set; } = null!;
    public string? SelectedColor { get; set; }
    public int Quantity { get; set; }
}
