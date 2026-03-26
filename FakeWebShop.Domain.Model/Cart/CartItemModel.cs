using System;

namespace FakeWebShop.Domain.Model.Cart;

public class CartItemModel
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}
