using System;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Variant.BaseColorVaria;

public abstract class BaseColorVariant
{
    [BsonElement("kleur")]
    public required string Kleur { get; set; }

    [BsonElement("imageUrl")]
    public required string ImageUrl { get; set; }
}
