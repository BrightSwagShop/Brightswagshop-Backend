using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FakeWebShop.Persistence.Entities.PublicUser;

public class User
{
      [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    //favorite array toevoegen van producten 
    //de array moet kunnen verschillende producten bevatten
    //de array moet van type product zijn en het moet favorites noemen
    //deze array behoudt productIds dat is beter omdat er geen duplicates zouden zijn 
    //!!!! hier productId toevoegen om te zorgen dat de mapping werkt maar hoe voeg ik de productId toe aan de de favorites array in de repo doe ik het eh ja 
    public string ProductId { get; set; }  = null!;
    public List<string> Favorites{ get; set; } = new();
    // eerst moet het leeg zijn wnr de public user op hartje klikt wordt de item toegevoegd aan de array 

}
