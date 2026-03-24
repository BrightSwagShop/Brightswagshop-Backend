using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Cart;

public class ShoppingCart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    public string? UserId { get; set; }

    [BsonElement("sessionId")]
    public string? SessionId { get; set; }

    [BsonElement("items")]
    public List<CartItem> Items { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("totalPrice")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice { get; set; }

    [BsonElement("subTotal")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal SubTotal { get; set; }
}
