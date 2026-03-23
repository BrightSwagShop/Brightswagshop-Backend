using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Discount;

public class DiscountItem
{
    [BsonElement("code")]
    public string Code { get; set; } = null!;

    [BsonElement("percentage")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Percentage { get; set; }

    [BsonElement("startsAt")]
    public DateTimeOffset StartsAt { get; set; }

    [BsonElement("endsAt")]
    public DateTimeOffset? EndsAt { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}
