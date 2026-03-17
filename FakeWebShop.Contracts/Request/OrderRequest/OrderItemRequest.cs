using System;

namespace FakeWebShop.Contracts.Request.OrderRequest;

public class OrderItemRequest
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
}
