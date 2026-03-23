using System;
using FakeWebShop.Domain.Model.Cart;

namespace FakeWebShop.Domain.Model.Discount;

public class DiscountModel
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }

    public decimal Percentage { get; set; }

    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset? EndsAt { get; private set; }
    public bool IsActive { get; private set; }

    public List<DiscountItemModel> Items { get; set; } = new List<DiscountItemModel>();

    public decimal CalculateDiscountFor(ShoppingCartModel cart, DateTimeOffset? now = null)
    {
        ArgumentNullException.ThrowIfNull(cart);

        var evaluationTime = now ?? DateTimeOffset.UtcNow;
        if (!IsCurrentlyValid(evaluationTime) || cart.Items.Count == 0)
        {
            return 0m;
        }

        var totalDiscount = 0m;

        foreach (var cartItem in cart.Items)
        {
            var lineTotal = cartItem.UnitPrice * cartItem.Quantity;

            var matchingDiscounts = Items
                .Where(item => item.IsCurrentlyValid(evaluationTime))
                .Select(item => item.Percentage)
                .ToList();

            if (matchingDiscounts.Count > 0)
            {
                var bestItemPercentage = matchingDiscounts.Max();
                totalDiscount += lineTotal * (bestItemPercentage / 100m);
                continue;
            }

            if (Percentage > 0)
            {
                totalDiscount += lineTotal * (Percentage / 100m);
            }
        }

        var cartTotal = cart.TotalPrice > 0
            ? cart.TotalPrice
            : cart.Items.Sum(item => item.UnitPrice * item.Quantity);

        return Math.Min(totalDiscount, cartTotal);
    }

    public void AddItem(DiscountItemModel item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Items.Add(item);
    }

    public bool IsCurrentlyValid(DateTimeOffset now) =>
        IsActive && now >= StartsAt && (EndsAt == null || now <= EndsAt);
}