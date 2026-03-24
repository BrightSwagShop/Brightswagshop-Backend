using System;

namespace FakeWebShop.Domain.Model.Cart;

public class ShoppingCartModel
{

    public string Id { get; set; } = null!;

    public string? UserId { get; set; }

    public string? SessionId { get; set; }

    public List<CartItemModel> Items { get; set; } = new();

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public decimal TotalPrice { get; set; }

    public decimal SubTotal { get; set; }
}
