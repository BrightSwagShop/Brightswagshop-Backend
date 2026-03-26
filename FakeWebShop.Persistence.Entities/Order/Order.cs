using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Persistence.Entities.Order;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    public string UserId { get; set; } = null!;

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;

    [BsonElement("paymentStatus")]
    [BsonRepresentation(BsonType.String)]
    public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;

    [BsonElement("paymentIntentId")]
    public string? PaymentIntentId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("totalPrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice { get; set; }
}