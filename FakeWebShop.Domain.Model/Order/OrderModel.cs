using System;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Domain.Model.Order;

public class OrderModel
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public List<OrderItemModel> Items { get; set; } = new();

    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;

    public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;

    public string? StripeCheckoutSessionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }
}
