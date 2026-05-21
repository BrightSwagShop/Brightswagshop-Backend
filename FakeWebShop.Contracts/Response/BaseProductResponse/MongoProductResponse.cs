using System;
using System.Text.Json.Serialization;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Contracts.Response.Products.BaseProductResponse;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ProductWithSizesResponse), "ProductWithSizes")]
[JsonDerivedType(typeof(SimpleProductResponse), "SimpleProduct")]
public abstract class MongoProductResponse
{
    public string Id { get; set; } = null!;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CategoryEnum Category { get; set; }
    public ProductTypeEnum ProductType { get; set; }
    public bool IsActive { get; set; }
}

