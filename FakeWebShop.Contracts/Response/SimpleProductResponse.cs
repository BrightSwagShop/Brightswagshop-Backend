using System;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Contracts.Response.VariantResponse;

namespace FakeWebShop.Contracts.Response;

public class SimpleProductResponse : MongoProductResponse
{
    public List<ColorVariantResponse> Kleuren { get; set; } = new();

}
