using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Persistence.Entities.Discount;

public class Discount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("percentage")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Percentage { get; set; }

    [BsonElement("code")]
    public string Code { get; set; } = null!;

    [BsonElement("startsAt")]
    public DateTimeOffset StartsAt { get; set; }

    [BsonElement("endsAt")]
    public DateTimeOffset? EndsAt { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}