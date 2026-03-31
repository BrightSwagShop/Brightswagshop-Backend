using System;
using FakeWebShop.Domain.Model.Cart;

namespace FakeWebShop.Domain.Model.Discount;

public class DiscountModel
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Percentage { get; set; }
    public string Code { get; set; } = string.Empty;

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public bool IsActive { get; set; }

    public decimal CalculateDiscountFor(ShoppingCartModel cart, DateTimeOffset? now = null)
    {
        ArgumentNullException.ThrowIfNull(cart);

        var evaluationTime = now ?? DateTimeOffset.UtcNow;
        if (!IsCurrentlyValid(evaluationTime) || cart.Items.Count == 0)
        {
            return 0m;
        }

        cart.TotalPrice = cart.Items.Sum(item => item.UnitPrice * item.Quantity);
        var totalDiscount = cart.TotalPrice * (Percentage / 100m);

        return totalDiscount;
    }

    public bool IsCurrentlyValid(DateTimeOffset now) =>
        IsActive && now >= StartsAt && (EndsAt == null || now <= EndsAt);
}