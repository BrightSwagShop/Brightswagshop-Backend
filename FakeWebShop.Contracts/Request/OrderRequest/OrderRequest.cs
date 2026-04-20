using System;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Contracts.Request.OrderRequest;

public class OrderRequest
{
    public string UserId { get; set; } = null!;
    public List<OrderItemRequest> Items { get; set; } = new();
}
