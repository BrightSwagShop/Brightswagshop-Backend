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

    public async Task UpdateAsync(string id, User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == id, user);
    }

}
