using FakeWebShop.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.BaseProduct;

// Geopteerd voor Polymorfisme
[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(ClothingProduct), typeof(MugProduct))]
public abstract class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    [BsonElement("category")]
    public CategoryEnum Category { get; set; }

    [BsonElement("productType")]
    public ProductTypeEnum ProductType { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}








