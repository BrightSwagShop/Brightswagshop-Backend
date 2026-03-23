using System;
using FakeWebShop.Domain.Model.Cart;

namespace FakeWebShop.Domain.Model.Discount;

public class DiscountItemModel
{
    public Guid Id { get; private set; }

    public Guid DiscountId { get; private set; }

    public string Code { get; set; } = string.Empty;

    public decimal Percentage { get; set; }

    public DateTimeOffset StartsAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? EndsAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsCurrentlyValid(DateTimeOffset now) =>
        IsActive && now >= StartsAt && (EndsAt == null || now <= EndsAt);

}