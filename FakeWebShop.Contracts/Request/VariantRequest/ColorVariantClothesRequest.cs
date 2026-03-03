using System;

namespace FakeWebShop.Contracts.Request.VariantRequest;

public class ColorVariantClothesRequest
{
    public required string Kleur { get; set; }
    public required string ImageUrl { get; set; }
    public List<SizeVariantRequest> Maten { get; set; } = new();
}
