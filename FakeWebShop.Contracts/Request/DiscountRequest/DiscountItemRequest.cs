using System;

namespace FakeWebShop.Contracts.Request.DiscountRequest;

public class DiscountItemRequest
{
    public string Code { get; set; } = null!;
    public string? ProductId { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
}