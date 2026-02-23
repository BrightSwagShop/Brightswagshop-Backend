using System;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Request.VariantRequest;

namespace FakeWebShop.Contracts.Request;

public class ClothingProductRequest : MongoProductRequest
{
    public List<ColorVariantClothesRequest> Kleuren { get; set; } = new();
}
