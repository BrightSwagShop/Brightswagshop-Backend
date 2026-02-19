using FakeWebShop.Persistence.Entities.BaseProduct;
using FakeWebShop.Persistence.Entities.Variant;
using MongoDB.Bson.Serialization.Attributes;


namespace FakeWebShop.Persistence.Entities;

[BsonDiscriminator("Mug")]
public class MugProduct : Product
{
    [BsonElement("kleuren")]
    public List<ColorVariant> Kleuren { get; set; } = new();
}
