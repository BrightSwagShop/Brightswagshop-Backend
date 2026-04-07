using System;
using FakeWebShop.Persistence.Entities.Favorites;
using MongoDB.Bson;

namespace FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

public interface IFavoriteRepository
{
    Task Add(FavoriteEntity entity);

    Task Remove(string userId, string productId);

    Task<List<FavoriteEntity>> GetByUser(string userId);

    Task<bool> Exists(string userId, string productId);

}
