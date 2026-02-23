using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Variant;

// Kleuren variant vooral voor kleren momenteel (ze hebben verschillende maten)
public class ColorVariantClothes
{

    [BsonElement("kleur")]
    public required string Kleur { get; set; }

    [BsonElement("imageUrl")]
    public required string ImageUrl { get; set; }

    [BsonElement("maten")]
    public List<SizeVariant> Maten { get; set; } = new();

}
