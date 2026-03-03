using System;

namespace FakeWebShop.Contracts.Response.VariantResponse;

public class ColorVariantClothesResponse
{
    public required string Kleur { get; set; }
    public required string ImageUrl { get; set; }
    public List<SizeVariantResponse> Maten { get; set; } = new();
}
