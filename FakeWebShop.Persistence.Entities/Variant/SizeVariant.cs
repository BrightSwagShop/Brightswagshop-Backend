using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Variant;

public class SizeVariant
{
    [BsonElement("maat")]
    public required string Maat { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("sku")]
    public required string Sku { get; set; } // Stock Keeping unit
}



