using System;
using FakeWebShop.Persistence.Entities.PublicUser;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.PublicUserRepo_s;

public class MongoUserRepository :IMongoUserRepository
{
     private readonly IMongoCollection<User> _users;

    public MongoUserRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var db = client.GetDatabase(options.Value.Database);
        _users = db.GetCollection<User>("users");
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }
    //voegFavoriteByUserAsync maken 
    // deze methode moet productId toevoegen aan de array van favorites bij user 
    // ik heb een andere methode nodig die de productId wegneemt van de array als de user terug klikt op
    //  de hartje met als bedoeling dat de product niet meer in zijn favoriten wil behouden
    //which user = userId
    //which product to like = productId
    public async Task VoegFavoriteByUserAsync(string userId, string productId)
    {
        //pak de favorites van deze user = u=>u.Favorites
        // u is een user, => doe dit met , u.Favorites = de favorites lijst van die user
        
        var update = Builders<User>.Update.AddToSet(u=>u.Favorites, productId);
        await _users.UpdateOneAsync(
            u =>u.Id == userId,
            update
            
        );
    }

    public async Task RemoveFavoriteAsync(string userId, string productId)
        {
            //.Update.Pull verwijder iets van dit array
            var update = Builders<User>.Update.Pull(u => u.Favorites, productId);

            await _users.UpdateOneAsync(
                u => u.Id == userId,
                update
            );
        }



    public async Task UpdateAsync(string id, User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == id, user);
    }

}
