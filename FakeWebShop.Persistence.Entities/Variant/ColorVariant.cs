using System;
using MongoDB.Bson.Serialization.Attributes;
using FakeWebShop.Persistence.Entities.Variant.BaseColorVaria;

namespace FakeWebShop.Persistence.Entities.Variant;

public class ColorVariant : BaseColorVariant
{
    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("sku")]
    public required string Sku { get; set; }
}