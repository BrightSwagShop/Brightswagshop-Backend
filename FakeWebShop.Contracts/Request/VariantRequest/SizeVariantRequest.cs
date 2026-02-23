using System;

namespace FakeWebShop.Contracts.Request.VariantRequest;

public class SizeVariantRequest
{
    public required string Maat { get; set; }
    public int Stock { get; set; }
    public required string Sku { get; set; } // Stock Keeping unit
}
