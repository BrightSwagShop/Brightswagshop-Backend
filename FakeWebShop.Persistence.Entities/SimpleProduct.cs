using FakeWebShop.Persistence.Entities.BaseProduct;
using FakeWebShop.Persistence.Entities.Variant;
using MongoDB.Bson.Serialization.Attributes;


namespace FakeWebShop.Persistence.Entities;

[BsonDiscriminator("Simple")]
public class SimpleProduct : Product
{
    [BsonElement("kleuren")]
    public List<ColorVariant> Kleuren { get; set; } = new();
}
