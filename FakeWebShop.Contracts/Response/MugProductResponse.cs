using System;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Contracts.Response.VariantResponse;

namespace FakeWebShop.Contracts.Response;

public class MugProductResponse : MongoProductResponse
{
    public List<ColorVariantResponse> Kleuren { get; set; } = new();

}
