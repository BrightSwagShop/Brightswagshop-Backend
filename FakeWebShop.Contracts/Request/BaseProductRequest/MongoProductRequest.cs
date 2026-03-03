using System;
using System.Text.Json.Serialization;
using FakeWebShop.Domain.Enums;


namespace FakeWebShop.Contracts.Request.Products.BaseProductRequest;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ClothingProductRequest), nameof(ProductTypeEnum.TShirt))]
[JsonDerivedType(typeof(MugProductRequest), nameof(ProductTypeEnum.Mok))]
public abstract class MongoProductRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CategoryEnum Category { get; set; }
    public ProductTypeEnum ProductType { get; set; }
    public bool IsActive { get; set; }
}
