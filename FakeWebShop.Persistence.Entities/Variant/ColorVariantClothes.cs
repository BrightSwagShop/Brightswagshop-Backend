using FakeWebShop.Persistence.Entities.Variant.BaseColorVaria;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Variant;

// Kleuren variant vooral voor kleren momenteel (ze hebben verschillende maten)
public class ColorVariantClothes : BaseColorVariant
{
    [BsonElement("maten")]
    public List<SizeVariant> Maten { get; set; } = new();
}
