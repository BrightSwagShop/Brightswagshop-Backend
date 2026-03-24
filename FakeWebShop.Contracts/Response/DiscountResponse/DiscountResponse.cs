using System;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Contracts.Response.DiscountResponse;

public class DiscountResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Percentage { get; set; }
    public string Code { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
}
