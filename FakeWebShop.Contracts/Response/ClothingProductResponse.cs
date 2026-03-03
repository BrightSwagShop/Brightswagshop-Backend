using System;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Contracts.Response.VariantResponse;

namespace FakeWebShop.Contracts.Response;

public class ClothingProductResponse : MongoProductResponse
{
    public List<ColorVariantClothesResponse> Kleuren { get; set; } = new();

}
