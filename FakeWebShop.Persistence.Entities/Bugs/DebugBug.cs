

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.Bugs;

public class DebugBug
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Key { get; set; }
    public bool Enabled { get; set; }
}
