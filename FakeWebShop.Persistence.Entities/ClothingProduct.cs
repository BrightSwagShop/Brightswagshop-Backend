using MongoDB.Bson.Serialization.Attributes;
using FakeWebShop.Persistence.Entities.BaseProduct;
using FakeWebShop.Persistence.Entities.Variant;


namespace FakeWebShop.Persistence.Entities;

// Erft over van Product
[BsonDiscriminator("Clothing")]
public class ClothingProduct : Product
{
    [BsonElement("kleuren")]
    public List<ColorVariantClothes> Kleuren { get; set; } = new();
}



