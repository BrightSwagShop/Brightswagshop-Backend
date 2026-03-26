using System;

namespace FakeWebShop.Domain.Model.Order;

public class OrderItemModel
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal => UnitPrice * Quantity;
}
