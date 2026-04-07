using System;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Domain.Model.favoritesModel;
using FakeWebShop.Domain.Services.MongoInterface_s;
using FakeWebShop.Persistence.Entities.Favorites;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using MongoDB.Bson;

namespace FakeWebShop.Domain.Services;

public class FavoriteService(IFavoriteRepository _repo) : IFavoriteService
{
      

    public async Task AddFavorite(string userId, string productId)
    {
        var entity = new FavoriteEntity
        {
            Id = Guid.NewGuid().ToString(), // geen ObjectId meer
            UserId = userId,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.Add(entity);
    }

    public async Task RemoveFavorite(string userId, string productId)
    {
        await _repo.Remove(userId, productId);
    }

    public async Task<List<FavoriteResponseContract>> GetFavorites(string userId)
    {
        var entities = await _repo.GetByUser(userId);

        return entities.Select(e => new FavoriteResponseContract
        {
            Id = e.Id,
            ProductId = e.ProductId,
            CreatedAt = e.CreatedAt
        }).ToList();
    }
}
