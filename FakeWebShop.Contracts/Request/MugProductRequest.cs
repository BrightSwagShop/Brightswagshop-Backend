using System;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Request.VariantRequest;

namespace FakeWebShop.Contracts.Request;

public class MugProductRequest : MongoProductRequest
{
    public List<ColorVariantRequest> Kleuren { get; set; } = new();

}
