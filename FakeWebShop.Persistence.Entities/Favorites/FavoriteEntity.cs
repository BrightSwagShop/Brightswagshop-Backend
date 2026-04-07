using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Favorites;

public class FavoriteEntity
{
  
    [BsonId]
     
    public string Id { get; set; } = null!;

    [BsonElement("userId")]
    
    public string UserId { get; set; } = null!;

    [BsonElement("productId")] 
    public string ProductId { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

}
