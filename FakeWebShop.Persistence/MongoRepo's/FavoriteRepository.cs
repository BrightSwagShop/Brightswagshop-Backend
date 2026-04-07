using System;
using FakeWebShop.Domain.Model.favoritesModel;
using FakeWebShop.Persistence.Entities.Favorites;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class FavoriteRepository: IFavoriteRepository
{
     private readonly IMongoCollection<FavoriteEntity> _collection;

    public FavoriteRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<FavoriteEntity>("favorites");
    }

    public async Task Add(FavoriteEntity entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Remove(string userId, string productId)
    {
        var filter = Builders<FavoriteEntity>.Filter.And(
            Builders<FavoriteEntity>.Filter.Eq(f => f.UserId, userId),
            Builders<FavoriteEntity>.Filter.Eq(f => f.ProductId, productId)
        );

        await _collection.DeleteOneAsync(filter);
    }

    public async Task<List<FavoriteEntity>> GetByUser(string userId)
    {
        return await _collection
            .Find(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> Exists(string userId, string productId)
    {
        var filter = Builders<FavoriteEntity>.Filter.And(
            Builders<FavoriteEntity>.Filter.Eq(f => f.UserId, userId),
            Builders<FavoriteEntity>.Filter.Eq(f => f.ProductId, productId)
        );

        return await _collection.Find(filter).AnyAsync();
    }
     
}
