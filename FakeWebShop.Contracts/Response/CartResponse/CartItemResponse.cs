using System;

namespace FakeWebShop.Contracts.Response.CartResponse;

public class CartItemResponse
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
