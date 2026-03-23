using System;

namespace FakeWebShop.Contracts.Response.DiscountResponse;

public class DiscountItemResponse
{
    public string Id { get; set; } = null!;
    public string DiscountId { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? ProductId { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
}