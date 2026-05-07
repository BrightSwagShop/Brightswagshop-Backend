using MongoDB.Bson.Serialization.Attributes;
using FakeWebShop.Persistence.Entities.BaseProduct;
using FakeWebShop.Persistence.Entities.Variant;


namespace FakeWebShop.Persistence.Entities;

// Erft over van Product
[BsonDiscriminator("WithSizes")]
public class ProductWithSizes : Product
{
    [BsonElement("kleuren")]
    public List<ColorVariantClothes> Kleuren { get; set; } = new();
}



