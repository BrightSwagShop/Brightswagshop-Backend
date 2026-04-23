using System;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Contracts.Response.OrderResponse;

public class OrderResponse
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public List<OrderItemResponse> Items { get; set; } = new();
    public OrderStatusEnum Status { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
}
