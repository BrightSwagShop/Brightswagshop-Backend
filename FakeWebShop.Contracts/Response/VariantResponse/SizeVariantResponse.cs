using System;

namespace FakeWebShop.Contracts.Response.VariantResponse;

public class SizeVariantResponse
{
    public required string Maat { get; set; }
    public int Stock { get; set; }
    public required string Sku { get; set; } // Stock Keeping unit
}
