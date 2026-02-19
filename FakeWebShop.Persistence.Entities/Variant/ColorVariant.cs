using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Variant;

// Kleuren variant wanneer ze geen Maten nodig hebben
public class ColorVariant
{
    [BsonElement("kleur")]
    public required string Kleur { get; set; }

    [BsonElement("imageUrl")]
    public required string ImageUrl { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("sku")]
    public required string Sku { get; set; } // Stock Keeping unit
}
