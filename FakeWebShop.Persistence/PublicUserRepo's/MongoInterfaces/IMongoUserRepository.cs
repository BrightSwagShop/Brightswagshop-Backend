using System;
using FakeWebShop.Persistence.Entities.PublicUser;

namespace FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;

public interface IMongoUserRepository
{
     Task<User> CreateAsync(User user);
    Task<User?> GetByUsernameAsync(string username);
    Task UpdateAsync(string id, User user);
    Task VoegFavoriteByUserAsync(string userId, string productId);
     Task RemoveFavoriteAsync(string userId, string productId);

}
